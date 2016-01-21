using System.Threading.Tasks;
using OCC.Passports.Common.Aspects;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Extensions;
using OCC.Passports.Common.Infrastructure;
using OCC.Passports.Common.Sample.Console.Contracts;

namespace OCC.Passports.Common.Sample.Console
{
    class ControllerUsingService : HasPassport
    {
        protected readonly IService Service;

        public ControllerUsingService(IPassport passport, IService service)
            : base(passport)
        {
            Service = service;
        }

        [UsePassport(sessionParameter: "session")]
        public async Task<StandardResponse<int>> Calculate(string session, int lhs, string @operator, int rhs)
        {
            return await this.ScopeAsync(async () => await Service.Calculate(Passport, lhs, @operator, rhs));
        }
    }
}
