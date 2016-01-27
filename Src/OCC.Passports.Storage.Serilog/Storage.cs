using OCC.Passports.Common;
using OCC.Passports.Common.Contracts.Services;
using System;
using System.Collections.Generic;
using OCC.Passports.Storage.Serilog.Extensions;
using Serilog;

namespace OCC.Passports.Storage.Serilog
{
    public class Storage : IPassportStorageService
    {
        private readonly ILogger _logger;

        public Storage(ILogger logger)
        {
            _logger = logger;
        }

        public void Store(Common.Infrastructure.MessageContext messageContext, IDictionary<string, object> snapshot)
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

            //var messageTemplate = "{SourceContext}:" + templateMessage;

            //var extendedData = new object[templateMessageData.Length + 1];
            //extendedData[0] = "THIS IS THE CLASS NAME";
            //Array.Copy(templateMessageData, 0, extendedData, 1, templateMessageData.Length);
        }

        public void Flush()
        {
            //throw new NotImplementedException();
        }
    }
}
