using System.Threading.Tasks;
using OCC.Passports.Common.Infrastructure;

namespace OCC.Passports.Common.Sample.Console.Contracts
{
    public interface IController
    {
        Task<StandardResponse<int>> Calculate(string session, int lhs, string @operator, int rhs);
    }
}
