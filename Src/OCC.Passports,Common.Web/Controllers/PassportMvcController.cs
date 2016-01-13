using System.Web.Mvc;
using OCC.Passports.Common.Contracts.Infrastructure;

namespace OCC.Passports.Common.Web.Controllers
{
    public abstract class PassportMvcController : Controller, IHasPassport
    {
        public IPassport Passport { get; private set; }

        protected PassportMvcController(IPassport passport)
        {
            Passport = passport;
        }
    }
}