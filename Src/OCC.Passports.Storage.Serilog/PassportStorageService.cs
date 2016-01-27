using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using OCC.Passports.Common;
using OCC.Passports.Common.Contracts.Services;
using System;
using System.Collections.Generic;
using OCC.Passports.Common.Domains;
using OCC.Passports.Common.Infrastructure.Contexts;
using OCC.Passports.Storage.Serilog.Extensions;
using Serilog;

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
            var currentContext = _logger.With(Constants.Passports.KeyCallerMemberName, messageContext.MemberName)
                                        .With(Constants.Passports.KeyCallerFilePath, messageContext.SourceFilePath)
                                        .With(Constants.Passports.KeyCallerLineNumber, messageContext.SourceLineNumber)
                                        .With(Constants.Passports.KeyPassport, messageContext.Passport)
                ;

            if (messageContext.Session != null)
            {
                currentContext = currentContext.With(Constants.Passports.KeySession, messageContext.Session);
            }

            if (!string.IsNullOrWhiteSpace(messageContext.EMail))
            {
                currentContext = currentContext.With(Constants.Passports.KeyEMail, messageContext.EMail);
            }

            if (snapshot != null
                && snapshot.Keys.Any())
            {
                foreach (var key in snapshot.Keys)
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
                        currentContext = currentContext.With(key, value);
                    //}
                }
            }

            var extendedMessageTemplate = "{SourceContext} " + messageContext.MessageTemplate;
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
