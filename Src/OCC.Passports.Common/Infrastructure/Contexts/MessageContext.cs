using System;
using OCC.Passports.Common.Domains;

namespace OCC.Passports.Common.Infrastructure.Contexts
{
    [Serializable]
    public class MessageContext
    {
        public object Session { get; set; }
        public Guid Passport { get; set; }

        public string MessageTemplate { get; set; }
        public object[] MessageTemplateParameters { get; set; }

        public PassportLevel Level { get; set; }
        public string EMail { get; set; }
        public string MemberName { get; set; }
        public string SourceFilePath { get; set; }
        public int SourceLineNumber { get; set; }
    }
}
