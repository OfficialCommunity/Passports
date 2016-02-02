using System.Threading.Tasks;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Extensions;
using OCC.Passports.Common.Infrastructure;
using OCC.Passports.Common.Sample.Console.Contracts;
using OCC.Passports.Common.Aspects;

namespace OCC.Passports.Common.Sample.Console
{
    class Service : IService
    {
        [LogParametersToPassport]
        public async Task<StandardResponse<int>> Calculate(IPassport passport, int lhs, string @operator, int rhs)
        {
            return await passport.ScopeAsync(async () =>
            {
                var result = 0;

                passport.Scope.Record(() => result, "First Recording");
                const string invalideOperatorMessage = "Invalid Operator";
                await Task.Delay(100);
                result = -1;
                passport.Scope.Record(() => result, "Second Recording");
                switch (@operator)
                {
                    case "/":
                        result = lhs / rhs;
                        break;
                    case "*":
                        result = lhs * rhs;
                        break;
                    case "-":
                        result = lhs - rhs;
                        break;
                    case "+":
                        result = lhs + rhs;
                        break;
                    default:
                        passport.Scope.Record(() => result, "Operation failed");
                        passport.Error(invalideOperatorMessage);
                        return result.GenerateStandardError(invalideOperatorMessage);
                }
                passport.Scope.Record(() => result, "Operation succeeded");
                return result.GenerateStandardResponse();
            });
        }

    }
}
