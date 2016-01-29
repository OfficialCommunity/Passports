using System.Threading.Tasks;
using OCC.Passports.Common.Aspects;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Extensions;
using OCC.Passports.Common.Infrastructure;

namespace OCC.Passports.Common.Sample.Console
{
    class Controller : HasPassport
    {
        public Controller(IPassport passport)
            : base(passport)
        {
        }

        [UsePassport(sessionParameter: "session")]
        public async Task<StandardResponse<int>> Calculate(string session, int lhs, string @operator, int rhs)
        {
            return await this.ScopeAsync(async () =>
            {
                var result = 0;

                const string invalideOperatorMessage = "Invalid Operator";
                await Task.Delay(100);
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
                        Passport.Scope.Record(() => result, "Operation failed");

                        Passport.Error(invalideOperatorMessage);
                        return result.GenerateStandardError(invalideOperatorMessage);
                }
                Passport.Scope.Record(() => result, "Operation succeeded");
                Passport.Debug("End of request");
                return result.GenerateStandardResponse();
            });
        }
    }
}
