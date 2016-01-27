using Newtonsoft.Json.Linq;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Contracts.Services;
using OCC.Passports.Common.Extensions;
using OCC.Passports.Common.Infrastructure;
using OCC.Passports.Common.Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OCC.Passports.Common.Domains
{
    public class Passport : IPassport
    {
        protected readonly IPassportStorageService PassportStorageService;

        protected readonly ThreadLocal<KeyValuePair<string, IContext>[]> CurrentContexts = new ThreadLocal<KeyValuePair<string, IContext>[]>(() => null);
        protected readonly ThreadLocal<string[]> CurrentScopes = new ThreadLocal<string[]>(() => null);

        protected readonly PassportsContext Contexts = new PassportsContext();

        public object SessionId { get; set; }
        public Guid PassportId { get; set; }
        public PassportScopeManager Scopes { get; set; }
        public PassportScope Scope { get; set; }

        public Passport(IPassportStorageService passportStorageService)
        {
            PassportStorageService = passportStorageService;
            Scopes = new PassportScopeManager();
            Scope = null;
            Contexts[Constants.Contexts.Machine] = new MachineContext();
        }

        protected virtual IEnumerable<KeyValuePair<string, IContext>> ExtendedContexts()
        {
            return null;
        }

        public virtual StandardResponse<bool> Stamp(MessageContext messageContext
                                                    , bool includeContext = false
                                                    , bool includeScopes = false
            )
        {
            if (PassportStorageService == null)
                return false.GenerateStandardResponse();

            try
            {
                var contexts = new PassportsContext();
                var scopes = new List<string>();

                if (includeContext)
                {
                    foreach (var context in Contexts)
                    {
                        contexts[context.Key] = context.Value;
                    }

                    var extendedContexts = ExtendedContexts();
                    if (extendedContexts != null)
                    {
                        foreach (var extendedContext in extendedContexts)
                        {
                            contexts[extendedContext.Key] = extendedContext.Value;
                        }
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

        public StandardResponse<bool> StampException(MessageContext messageContext, Exception e)
        {
            if (PassportStorageService == null || e == null)
                return false.GenerateStandardResponse();

            try
            {
                var contexts = new PassportsContext();
                var scopes = new List<string>();

                foreach (var context in Contexts)
                {
                    contexts[context.Key] = context.Value;
                }
                
                var extendedContexts = ExtendedContexts();
                if (extendedContexts != null)
                {
                    foreach (var extendedContext in extendedContexts)
                    {
                        contexts[extendedContext.Key] = extendedContext.Value;
                    }
                }

                contexts[Constants.Contexts.Exception] = new ExceptionContext(e);

                scopes.AddRange(Scopes.Serialize());

                SendInBackground(messageContext, contexts.ToArray(), scopes.ToArray());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Failed SendInBackground: {0}", ex.Message);
                return false.GenerateStandardResponse();
            }

            return true.GenerateStandardResponse();
        }

        public void Send(MessageContext messageContext)
        {
            var snapshot = new Dictionary<string, object>();
            if (CurrentContexts.IsValueCreated)
            {
                if (CurrentContexts.Value.Any())
                {
                    foreach (var context in CurrentContexts.Value)
                    {
                        snapshot[context.Key] = context.Value;
                    }
                }
            }

            if (CurrentScopes.IsValueCreated)
            {
                if (CurrentScopes.Value.Any())
                {
                    var scopeEntries = new JArray();
                    scopeEntries = CurrentScopes.Value.Select(JArray.Parse).Aggregate(scopeEntries, (current, entries) => new JArray(current.Union(entries)));
                    snapshot[Constants.Passports.KeyScopes] = scopeEntries.OrderBy(x => (DateTimeOffset)x[Constants.PassportScope.Entry.Timestamp])
                        .ToList();
                }
            }

            PassportStorageService.Store(messageContext, snapshot);
        }

        protected void SendInBackground(dynamic messageContext
                                            , KeyValuePair<string, IContext>[] contexts
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
    }
}