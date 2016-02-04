﻿using System;
using System.Diagnostics;
using Newtonsoft.Json;
using OCC.Passports.Common.Domains;

namespace OCC.Passports.Common.Infrastructure.Contexts
{
    [Serializable]
    public class MessageContext
    {
        public MessageContext()
        {
            Timestamp = new DateTimeOffset(new DateTime(Stopwatch.GetTimestamp()).ToUniversalTime());
            Id = Guid.NewGuid();
        }

        [JsonProperty(Order = 0)]
        public DateTimeOffset Timestamp { get; private set; }

        [JsonProperty(Order = 1)]
        public string Member { get; set; }

        [JsonProperty(Order = 2)]
        public string CallContext { get; set; }

        [JsonProperty(Order = 3)]
        public string ParentContext { get; set; }

        [JsonProperty(Order = 4)]
        public object Session { get; set; }
        
        [JsonProperty(Order = 5)]
        public Guid Passport { get; set; }
        [JsonProperty(Order = 6)]
        public string User { get; set; }

        [JsonProperty(Order = 7)]
        public string MessageTemplate { get; set; }
        [JsonProperty(Order = 8)]
        public object[] MessageTemplateParameters { get; set; }

        [JsonProperty(Order = 9)]
        public PassportLevel Level { get; set; }

        [JsonProperty(Order = 10)]
        public bool LogCaller { get; set; }

        [JsonProperty(Order = 11)]
        public string MemberName { get; set; }
        [JsonProperty(Order = 12)]
        public string SourceFilePath { get; set; }
        [JsonProperty(Order = 13)]
        public int SourceLineNumber { get; set; }

        [JsonProperty(Order = 14)]
        public int ScopeDepth { get; set; }

        [JsonProperty(Order = 15)]
        public Guid Id { get; private set; }
    }
}
