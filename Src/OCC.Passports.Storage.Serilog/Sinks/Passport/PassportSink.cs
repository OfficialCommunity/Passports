using System;
using System.Linq;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace OCC.Passports.Storage.Serilog.Sinks.Passport
{
    public class PassportSink : ILogEventSink
    {
        private readonly ILogger _destination;

        public PassportSink(ILogger destination) 
        {
            _destination = destination;
        }

        public void Emit(LogEvent logEvent)
        {
            var correctedLogEvent = logEvent;
            if (logEvent.Properties.ContainsKey(Common.Constants.KeyPassportTimestamp))
            {
                var passportTimestampProperty = logEvent.Properties[Common.Constants.KeyPassportTimestamp];
                if (passportTimestampProperty != null)
                {
                    var passportTimestampValue = passportTimestampProperty as ScalarValue;
                    if (passportTimestampValue != null)
                    {
                        if (passportTimestampValue.Value is DateTimeOffset)
                        {
                            var passportTimestamp = (DateTimeOffset)passportTimestampValue.Value;
                            correctedLogEvent = new LogEvent(passportTimestamp
                                                                , logEvent.Level
                                                                , logEvent.Exception
                                                                , logEvent.MessageTemplate
                                                                , logEvent.Properties.Select(x => new LogEventProperty(x.Key, x.Value)).ToList()
                                                                );
                            correctedLogEvent.RemovePropertyIfPresent(Common.Constants.KeyPassportTimestamp);
                        }
                    }
                }
            }

            _destination.Write(correctedLogEvent);
        }
    }
}
