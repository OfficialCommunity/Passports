using System.Threading.Tasks;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Infrastructure;

namespace OCC.Passports.Common.Sample.Console.Contracts
{
    public interface IService
    {
        Task<StandardResponse<int>> Calculate(IPassport passport, int lhs, string @operator, int rhs);
    }
}