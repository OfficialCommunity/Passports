using OCC.Passports.Common.Contracts.Infrastructure;

namespace OCC.Passports.Common.Infrastructure
{
    public class HasPassport : IHasPassport
    {
        public IPassport Passport { get; private set; }

        public HasPassport(IPassport passport)
        {
            Passport = passport;
        }
    }
}
