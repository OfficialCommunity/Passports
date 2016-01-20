using OCC.Passports.Common.Aspects;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Extensions;
using OCC.Passports.Common.Infrastructure;

namespace OCC.Passports.Common.Tests.Models
{
    public class UserPassportAttributeClass : HasPassport
    {
        public UserPassportAttributeClass(IPassport passport)
            : base(passport)
        {
            
        }

        [UsePassport]
        public StandardResponse<bool> NoSession()
        {
            return true.GenerateStandardResponse();
        }

        [UsePassport(sessionParameter: "session")]
        public StandardResponse<bool> ValidSession(string session)
        {
            return true.GenerateStandardResponse();
        }

        [UsePassport(sessionParameter: "invalidsession")]
        public StandardResponse<bool> InvalidSession(string session)
        {
            return true.GenerateStandardResponse();
        }
    }
}
