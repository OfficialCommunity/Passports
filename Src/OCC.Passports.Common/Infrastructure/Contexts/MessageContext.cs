using System;
using Newtonsoft.Json;
using OCC.Passports.Common.Domains;

namespace OCC.Passports.Common.Infrastructure.Contexts
{
    [Serializable]
    public class MessageContext
    {
        public MessageContext()
        {
            Timestamp = new DateTimeOffset(DateTime.UtcNow);
        }

        [JsonProperty(Order = 0)]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty(Order = 1)]
        public string SourceContext { get; set; }

        [JsonProperty(Order = 2)]
        public object Session { get; set; }
        [JsonProperty(Order = 3)]
        public Guid Passport { get; set; }
        [JsonProperty(Order = 4)]
        public string User { get; set; }


        [JsonProperty(Order = 5)]
        public string MessageTemplate { get; set; }
        [JsonProperty(Order = 6)]
        public object[] MessageTemplateParameters { get; set; }

        [JsonProperty(Order = 7)]
        public PassportLevel Level { get; set; }

        [JsonProperty(Order = 8)]
        public string MemberName { get; set; }
        [JsonProperty(Order = 9)]
        public string SourceFilePath { get; set; }
        [JsonProperty(Order = 10)]
        public int SourceLineNumber { get; set; }

        [JsonProperty(Order = 11)]
        public Guid ScopeId { get; set; }
    }
}
