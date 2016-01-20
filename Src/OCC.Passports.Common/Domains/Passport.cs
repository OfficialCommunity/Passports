using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Contracts.Services;
using OCC.Passports.Common.Extensions;
using OCC.Passports.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace OCC.Passports.Common.Domains
{
    public class Passport : IPassport
    {
        protected readonly IPassportStorageService PassportStorageService;

        protected readonly ThreadLocal<KeyValuePair<string, dynamic>[]> CurrentContexts = new ThreadLocal<KeyValuePair<string, dynamic>[]>(() => null);
        protected readonly ThreadLocal<string[]> CurrentScopes = new ThreadLocal<string[]>(() => null);

        protected readonly dynamic Context;

        public object SessionId { get; set; }
        public Guid PassportId { get; set; }
        public PassportScopeManager Scopes { get; set; }
        public PassportScope Scope { get; set; }

        public Passport(IPassportStorageService passportStorageService)
        {
            PassportStorageService = passportStorageService;
            Scopes = new PassportScopeManager();
            Scope = null;
            Context = BuildImmutableContext();
        }

        private static dynamic BuildImmutableContext()
        {
            dynamic context = new ExpandoObject();

            try
            {
                var now = DateTime.Now;
                context.UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(now).TotalHours;
                context.Locale = CultureInfo.CurrentCulture.DisplayName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Error retrieving time and locale: {0}", ex.Message);
            }

            return context;
        }

        protected virtual IEnumerable<KeyValuePair<string, dynamic>> ExtendedContexts()
        {
            return null;
        }

        public virtual StandardResponse<bool> Stamp(dynamic messageContext
                                                    , bool includeContext = false
                                                    , bool includeScopes = false
            )
        {
            if (PassportStorageService == null)
                return false.GenerateStandardResponse();

            try
            {
                var contexts = new List<KeyValuePair<string, dynamic>>();
                var scopes = new List<string>();

                if (includeContext)
                {
                    contexts.Add(new KeyValuePair<string, dynamic>(Constants.Passports.KeyCallContext, Context));

                    var extendedContexts = ExtendedContexts();
                    if (extendedContexts != null)
                    {
                        contexts.AddRange(extendedContexts);
                    }
                }

                if (includeScopes)
                {
                    scopes.AddRange(Scopes.Serialize());
                }

                SendInBackground(messageContext, contexts.ToArray(), scopes.ToArray());
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Failed SendInBackground: {0}", e.Message);
                return false.GenerateStandardResponse();
            }

            return true.GenerateStandardResponse();
        }

        public StandardResponse<bool> StampException(Exception e)
        {
            if (PassportStorageService == null || e == null)
                return false.GenerateStandardResponse();

            try
            {
                var contexts = new List<KeyValuePair<string, dynamic>>();
                var scopes = new List<string>();

                contexts.Add(new KeyValuePair<string, dynamic>(Constants.Passports.KeyCallContext, Context));

                var extendedContexts = ExtendedContexts();
                if (extendedContexts != null)
                {
                    contexts.AddRange(extendedContexts);
                }
                scopes.AddRange(Scopes.Serialize());

                dynamic context = new ExpandoObject();
                var messageContext = context as IDictionary<string, Object>;

                if (SessionId != null)
                    messageContext[Constants.Passports.KeySession] = SessionId;

                messageContext[Constants.Passports.KeyPassport] = PassportId;

                messageContext[Constants.Passports.KeyTimestamp] = new DateTimeOffset(DateTime.UtcNow);
                messageContext[Constants.Passports.KeyLevel] = Constants.PassportLevel.Exception;

                BuildException(context, e);

                SendInBackground(messageContext, contexts.ToArray(), scopes.ToArray());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Failed SendInBackground: {0}", ex.Message);
                return false.GenerateStandardResponse();
            }

            return true.GenerateStandardResponse();
        }

        public void Send(dynamic messageContext)
        {
            var dictionary = messageContext as IDictionary<string, object>;
            if (dictionary != null)
            {
                if (CurrentContexts.IsValueCreated)
                {
                    if (CurrentContexts.Value.Any())
                    {
                        foreach (var context in CurrentContexts.Value)
                        {
                            dictionary[context.Key] = context.Value;
                        }
                    }
                }

                if (CurrentScopes.IsValueCreated)
                {
                    if (CurrentScopes.Value.Any())
                    {
                        var scopeEntries = new JArray();
                        scopeEntries = CurrentScopes.Value.Select(JArray.Parse).Aggregate(scopeEntries, (current, entries) => new JArray(current.Union(entries)));
                        dictionary[Constants.Passports.KeyScopes] = scopeEntries.OrderBy(x => (DateTimeOffset)x[Constants.PassportScope.Entry.Timestamp])
                                                                                .ToList();
                    }
                }
            }

            PassportStorageService.Store(messageContext);
        }

        protected void SendInBackground(dynamic messageContext
                                            , KeyValuePair<string, dynamic>[] contexts
                                            , string[] scopes)
        {
            ThreadPool.QueueUserWorkItem(c =>
            {
                CurrentContexts.Value = contexts;
                CurrentScopes.Value = scopes;
                Send(messageContext);
            });
        }

        public void PushScope(string name)
        {
            Scope = Scopes.Push(name);
        }

        public void PopScope()
        {
            Scope = Scopes.Pop();
        }

        private static void BuildException(dynamic context, Exception exception)
        {
            dynamic exceptionContext = new ExpandoObject();

            var exceptionType = exception.GetType();

            exceptionContext.Message = exception.Message;
            exceptionContext.ClassName = FormatTypeName(exceptionType, true);
            exceptionContext.StackTrace = BuildStackTrace(exception);

            var innerExceptions = GetInnerExceptions(exception);
            if (innerExceptions != null && innerExceptions.Count > 0)
            {
                exceptionContext.InnerErrors = new ExpandoObject[innerExceptions.Count];
                var index = 0;
                foreach (var e in innerExceptions)
                {
                    exceptionContext.InnerErrors[index] = BuildException(exceptionContext, e);
                    index++;
                }
            }
            else if (exception.InnerException != null)
            {
                exceptionContext.InnerError = BuildException(exceptionContext, exception.InnerException);
            }

            context.Exception = exception;
        }

        private static IList<Exception> GetInnerExceptions(Exception exception)
        {
            var ae = exception as AggregateException;
            if (ae != null)
            {
                return ae.InnerExceptions;
            }

            var rtle = exception as ReflectionTypeLoadException;
            if (rtle == null) return null;
            var index = 0;
            foreach (var e in rtle.LoaderExceptions)
            {
                try
                {
                    e.Data["Type"] = rtle.Types[index];
                }
                catch
                {

                }
                index++;
            }

            return rtle.LoaderExceptions.ToList();
        }

        protected static string FormatTypeName(Type type, bool fullName)
        {
            var name = fullName ? type.FullName : type.Name;
            if (!type.IsGenericType)
            {
                return name;
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(name.Substring(0, name.IndexOf("`", StringComparison.Ordinal)));
            stringBuilder.Append("<");
            foreach (var t in type.GetGenericArguments())
            {
                stringBuilder.Append(FormatTypeName(t, false)).Append(",");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append(">");

            return stringBuilder.ToString();
        }

        private static StackTraceLineMessage[] BuildStackTrace(Exception exception)
        {
            var lines = new List<StackTraceLineMessage>();

            var stackTrace = new StackTrace(exception, true);
            var frames = stackTrace.GetFrames();

            if (frames == null || frames.Length == 0)
            {
                var line = new StackTraceLineMessage { FileName = "none", LineNumber = 0 };
                lines.Add(line);
                return lines.ToArray();
            }

            foreach (var frame in frames)
            {
                var method = frame.GetMethod();

                if (method == null) continue;

                var lineNumber = frame.GetFileLineNumber();

                if (lineNumber == 0)
                {
                    lineNumber = frame.GetILOffset();
                }

                var methodName = GenerateMethodName(method);

                var file = frame.GetFileName();

                var className = method.ReflectedType != null ? method.ReflectedType.FullName : "(unknown)";

                var line = new StackTraceLineMessage
                {
                    FileName = file,
                    LineNumber = lineNumber,
                    MethodName = methodName,
                    ClassName = className
                };

                lines.Add(line);
            }

            return lines.ToArray();
        }

        protected static string GenerateMethodName(MethodBase method)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(method.Name);

            bool first = true;
            if (method is MethodInfo && method.IsGenericMethod)
            {
                var genericArguments = method.GetGenericArguments();
                stringBuilder.Append("[");
                foreach (var t in genericArguments)
                {
                    if (!first)
                    {
                        stringBuilder.Append(",");
                    }
                    else
                    {
                        first = false;
                    }
                    stringBuilder.Append(t.Name);
                }
                stringBuilder.Append("]");
            }
            stringBuilder.Append("(");
            var parameters = method.GetParameters();
            first = true;
            foreach (var t in parameters)
            {
                if (!first)
                {
                    stringBuilder.Append(", ");
                }
                else
                {
                    first = false;
                }
                var type = "<UnknownType>";
                if (t.ParameterType != null)
                {
                    type = t.ParameterType.Name;
                }
                stringBuilder.Append(type + " " + t.Name);
            }
            stringBuilder.Append(")");

            return stringBuilder.ToString();
        }

        public class StackTraceLineMessage
        {
            public int LineNumber { get; set; }

            public string ClassName { get; set; }

            public string FileName { get; set; }

            public string MethodName { get; set; }
        }
    }
}