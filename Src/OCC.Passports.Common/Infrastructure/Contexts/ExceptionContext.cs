using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using OCC.Passports.Common.Contracts.Infrastructure;

namespace OCC.Passports.Common.Infrastructure.Contexts
{
    public class ExceptionContext : IContext
    {
        public class StackTraceLineMessage
        {
            public int LineNumber { get; set; }

            public string ClassName { get; set; }

            public string FileName { get; set; }

            public string MethodName { get; set; }

            public override string ToString()
            {
                return string.Format("{0}:{1}:{2}:{3}"
                    , FileName
                    , ClassName
                    , MethodName
                    , LineNumber
                    );
            }
        }

        public ExceptionContext(Exception e)
        {
            var exceptionType = e.GetType();

            Message = e.Message;
            ClassName = FormatTypeName(exceptionType, true);
            HResult = e.HResult;
            HelpUrl = e.HelpLink;

            StackTrace = BuildStackTrace(e);

            var innerExceptions = GetInnerExceptions(e);
            if (innerExceptions != null && innerExceptions.Count > 0)
            {
                InnerExceptions = new ExceptionContext[innerExceptions.Count];
                var index = 0;
                foreach (var innerException in innerExceptions)
                {
                    InnerExceptions[index] = new ExceptionContext(innerException);
                    index++;
                }
            }
            else if (e.InnerException != null)
            {
                InnerExceptions = new ExceptionContext[1];
                InnerExceptions[0] = new ExceptionContext(e.InnerException);
            }
        }

        public string Message { get; private set; }
        public string ClassName { get; private set; }
        public int HResult { get; private set; }
        public string HelpUrl { get; private set; }
        public string[] StackTrace { get; private set; }
        public ExceptionContext[] InnerExceptions { get; private set; }

        private static string FormatTypeName(Type type, bool fullName)
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

        private static string[] BuildStackTrace(Exception exception)
        {
            var lines = new List<StackTraceLineMessage>();

            var stackTrace = new StackTrace(exception, true);
            var frames = stackTrace.GetFrames();

            if (frames == null || frames.Length == 0)
            {
                var line = new StackTraceLineMessage { FileName = "none", LineNumber = 0 };
                lines.Add(line);
                return lines.Select(x => x.ToString()).ToArray();
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

            return lines.Select(x => x.ToString()).ToArray();
        }

        protected static string GenerateMethodName(MethodBase method)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(method.Name);

            var first = true;
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
                var type = t.ParameterType.Name;
                stringBuilder.Append(type + " " + t.Name);
            }
            stringBuilder.Append(")");

            return stringBuilder.ToString();
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
                catch (Exception)
                {

                }
                index++;
            }

            return rtle.LoaderExceptions.ToList();
        }

    }
}