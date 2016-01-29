using System.Linq;
using Newtonsoft.Json.Linq;
using OCC.Passports.Common.Contracts.Services;
using System;
using System.Collections.Generic;
using OCC.Passports.Common.Domains;
using OCC.Passports.Common.Infrastructure.Contexts;
using OCC.Passports.Storage.Serilog.Extensions;
using Serilog;
using Serilog.Core;
using Serilog.Events;
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
            var currentContext = _logger.With("E_1_SourceContext", messageContext.SourceContext)
                                        .With("E_7_" + Constants.Passports.KeyCallerMemberName, messageContext.MemberName)
                                        .With("E_8_" + Constants.Passports.KeyCallerFilePath, messageContext.SourceFilePath)
                                        .With("E_9_" + Constants.Passports.KeyCallerLineNumber, messageContext.SourceLineNumber)
                                        .With("E_3_" + Constants.Passports.KeyPassport, messageContext.Passport)
                                        .With("CurrentScope", messageContext.ScopeId)
             ;

            if (messageContext.Session != null)
            {
                currentContext = currentContext.With("E_2_" + Constants.Passports.KeySession, messageContext.Session);
            }

            if (!string.IsNullOrWhiteSpace(messageContext.User))
            {
                currentContext = currentContext.With("E_4_" + Constants.Passports.KeyUser, messageContext.User);
            }

            if (snapshot != null
                && snapshot.Keys.Any())
            {
                foreach (var key in snapshot.Keys.Where(x => x != Constants.Passports.KeyScopes))
                {
                    var value = snapshot[key];
                    //if (value is ExpandoObject)
                    //{
                    //    dynamic dynamicValue = value;
                    //    currentContext = currentContext.ForContext(key, JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(dynamicValue)));
                    //}
                    //else if (value is JToken)
                    //{
                        
                    //}
                    //else
                    //{
                    currentContext = currentContext.With("E_6_" + key, value);
                    //}
                }

                if (snapshot.ContainsKey(Constants.Passports.KeyScopes))
                {
                    try
                    {
                        var entries = (IList<JToken>) snapshot[Constants.Passports.KeyScopes];
                        if (entries != null)
                        {
                            var entryCounter = 0;

                            foreach (var entry in entries)
                            {
                                var entryKey = string.Format("E_{0}_{1}", Constants.Passports.KeyScopesShort, entryCounter++);
                                currentContext = currentContext.With(entryKey, new {
                                    Name = (string)entry[Constants.PassportScope.Entry.Name],
                                    Timestamp = (DateTimeOffset)entry[Constants.PassportScope.Entry.Timestamp]
                                });

                                var historyCounter = 0;
                                var history = entry[Constants.PassportScope.Entry.History].Children();
                                foreach (var record in history)
                                {
                                    var recordName = (string) record[Constants.PassportScope.Entry.Name];
                                    var recordKey = string.Format("{0}_{1}_{2}", entryKey, historyCounter++, recordName);
                                    currentContext = currentContext.With(recordKey, record);
                                }
                            }
                        }
                    }
                    catch 
                    {
                        
                    }
                }
            }

            var extendedMessageTemplate = "{E_1_SourceContext} " + messageContext.MessageTemplate;
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
            extendedMessageTemplateParameters[0] = messageContext.SourceContext;
   
            if ( messageContext.Level == PassportLevel.Debug)
            {
                //currentContext.Write(new LogEvent(messageContext.Timestamp,LogEventLevel.Debug, null, extendedMessageTemplate, 
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
    }
}
