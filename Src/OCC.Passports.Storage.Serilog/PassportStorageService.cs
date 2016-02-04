using Newtonsoft.Json.Linq;
using OCC.Passports.Common.Contracts.Services;
using OCC.Passports.Common.Domains;
using OCC.Passports.Common.Infrastructure.Contexts;
using OCC.Passports.Storage.Serilog.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Constants = OCC.Passports.Common.Constants;

namespace OCC.Passports.Storage.Serilog
{
    public class PassportStorageService : IPassportStorageService
    {
        private readonly ILogger _logger;

        public PassportStorageService(ILogger logger)
        {
            _logger = logger;
        }

        public void Store(MessageContext messageContext, IDictionary<string, object> snapshot)
        {
            var currentContext = _logger.With(Constants.KeyPassportTimestamp, messageContext.Timestamp)
                                        .With("E_0_" + Constants.Passports.KeyMember, messageContext.Member)                        
                                        .With("E_1_" + Constants.Passports.KeyCurrentCallContext, messageContext.CallContext)
                                        .With("E_3_" + Constants.Passports.KeyPassport, messageContext.Passport)
                                        
             ;

            if (!string.IsNullOrWhiteSpace(messageContext.ParentContext))
            {
                currentContext = currentContext.With("E_2_" + Constants.Passports.KeyParentCallContext,messageContext.ParentContext);
            }

            if (messageContext.LogCaller)
            {
                currentContext = currentContext.With("Z_1_"+Constants.Passports.KeyCallerFilePath, messageContext.SourceFilePath)
                    .With("Z_2_"+Constants.Passports.KeyCallerLineNumber, messageContext.SourceLineNumber)
                    ;
            }

            if (messageContext.Session != null)
            {
                currentContext = currentContext.With("E_4_" + Constants.Passports.KeySession, messageContext.Session);
            }

            if (!string.IsNullOrWhiteSpace(messageContext.User))
            {
                currentContext = currentContext.With("E_5_" + Constants.Passports.KeyUser, messageContext.User);
            }

            if (snapshot != null
                && snapshot.Keys.Any())
            {
                var contextCount = 0;
                foreach (var key in snapshot.Keys.Where(x => x != Constants.Passports.KeyScopes))
                {
                    var value = snapshot[key];
                    currentContext = currentContext.With(string.Format("C_{0}_{1}",contextCount++, key), value);
                }

                if (snapshot.ContainsKey(Constants.Passports.KeyScopes))
                {
                    try
                    {
                        var entries = (IList<JToken>) snapshot[Constants.Passports.KeyScopes];
                        if (entries != null && entries.Any())
                        {
                            if (messageContext.ScopeDepth == 0)
                            {
                                var lastEntry = entries.LastOrDefault();
                                if (lastEntry != null)
                                {
                                    currentContext = BuildScopeEntry(currentContext, lastEntry, 0);
                                }
                            }
                            else
                            {
                                var calculatedDepth = messageContext.ScopeDepth == -1
                                    ? int.MaxValue
                                    : messageContext.ScopeDepth;

                                var entryCounter = 0;

                                currentContext = entries.Reverse()
                                                        .Where(entry => entryCounter < calculatedDepth)
                                                        .Aggregate(currentContext, (current, entry) => BuildScopeEntry(current, entry, entryCounter++));
                            }
                        }
                    }
                    catch 
                    {
                        
                    }
                }
            }

            var extendedMessageTemplate = "{" + "E_0_" + Constants.Passports.KeyMember  +"} " + messageContext.MessageTemplate;
            object[] extendedMessageTemplateParameters = null;
            if (messageContext.MessageTemplateParameters != null)
            {
                extendedMessageTemplateParameters = new object[messageContext.MessageTemplateParameters.Length + 1];
                Array.Copy(messageContext.MessageTemplateParameters, 0, extendedMessageTemplateParameters, 1, messageContext.MessageTemplateParameters.Length);
            }
            else
            {
                extendedMessageTemplateParameters = new object[1];
            }
            extendedMessageTemplateParameters[0] = messageContext.Member;

            if ( messageContext.Level == PassportLevel.Debug)
            {
                currentContext.Debug(extendedMessageTemplate, extendedMessageTemplateParameters);
            }
            else if (messageContext.Level == PassportLevel.Info)
            {
                currentContext.Information(extendedMessageTemplate, extendedMessageTemplateParameters);
            }
            else if (messageContext.Level == PassportLevel.Warn)
            {
                currentContext.Warning(extendedMessageTemplate, extendedMessageTemplateParameters);
            }
            else if (messageContext.Level == PassportLevel.Error)
            {
                currentContext.Error(extendedMessageTemplate, extendedMessageTemplateParameters);
            }
            else if (messageContext.Level == PassportLevel.Exception)
            {
                currentContext.Fatal(extendedMessageTemplate, extendedMessageTemplateParameters);
            }
        }

        public void Flush()
        {
            //throw new NotImplementedException();
        }

        private ILogger BuildScopeEntry(ILogger currentContext, JToken entry, int position)
        {
            var entryKey = string.Format("E_{0}_{1}", Constants.Passports.KeyScopesShort, position);
            currentContext = currentContext.With(entryKey, new
            {
                Name = (string)entry[Constants.PassportScope.Entry.Name],
                Timestamp = (DateTimeOffset)entry[Constants.PassportScope.Entry.Timestamp]
            });

            var historyCounter = 0;
            var history = entry[Constants.PassportScope.Entry.History].Children();
            foreach (var record in history)
            {
                var recordName = (string)record[Constants.PassportScope.Entry.Name];
                var recordKey = string.Format("{0}_{1}_{2}", entryKey, historyCounter++,
                    recordName);
                currentContext = currentContext.With(recordKey, record);
            }

            return currentContext;
        }

        //public LogEvent LogEvent(MessageContext messageContext, Exception exception = null)
        //{
        //    if (messageContext.MessageTemplate == null) return null;
            
        //    // Catch a common pitfall when a single non-object array is cast to object[]
        //    if (messageContext.MessageTemplateParameters != null &&
        //        messageContext.MessageTemplateParameters.GetType() != typeof(object[]))
        //        messageContext.MessageTemplateParameters = new object[] { messageContext.MessageTemplateParameters };

        //    var now = DateTimeOffset.Now;

        //    MessageTemplate parsedTemplate;
        //    IEnumerable<LogEventProperty> properties;

        //    _messageTemplateProcessor.Process(messageContext.MessageTemplate, messageContext.MessageTemplateParameters, out parsedTemplate, out properties);

        //    var logLevel = LogEventLevel.Information; 
        //    if (messageContext.Level == PassportLevel.Debug)
        //    {
        //        logLevel = LogEventLevel.Debug;
        //    }
        //    else if (messageContext.Level == PassportLevel.Info)
        //    {
        //        logLevel = LogEventLevel.Information;
        //    }
        //    else if (messageContext.Level == PassportLevel.Warn)
        //    {
        //        logLevel = LogEventLevel.Warning;
        //    }
        //    else if (messageContext.Level == PassportLevel.Error)
        //    {
        //        logLevel = LogEventLevel.Error;
        //    }
        //    else if (messageContext.Level == PassportLevel.Exception)
        //    {
        //        logLevel = LogEventLevel.Error;
        //    }

        //    var logEvent = new LogEvent(now, logLevel, exception, parsedTemplate, properties);

        //    return logEvent;
        //}
    }
}
