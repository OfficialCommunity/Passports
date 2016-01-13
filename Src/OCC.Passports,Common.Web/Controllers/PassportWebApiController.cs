using System.Web.Http;
using OCC.Passports.Common.Contracts.Infrastructure;

namespace OCC.Passports.Common.Web.Controllers
{
    public abstract class PassportWebApiController : ApiController, IHasPassport
    {
        public IPassport Passport { get; private set; }

        protected PassportWebApiController(IPassport passport)
        {
            Passport = passport;
        }
    }
}
