using System.Linq;
using System.Runtime.InteropServices;
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

        [UsePassport(sessionParameter: "session")]
        public async Task<StandardResponse<bool>> AdditionTest(string session)
        {
            return await this.ScopeAsync(async () =>
            {
                await Task.Delay(100);

                var ids = new[]
                {
                    1, 
                    2, 
                    //3, 4, 5
                };

                await Task.WhenAll(ids.Select(x => Add1Then100(Passport,x)));

                return true.GenerateStandardResponse();
            });
        }

        [LogParametersToPassport]
        public async Task<StandardResponse<bool>> Add1Then100(IPassport passport,int value)
        {
            return await this.ScopeAsync(async () =>
            {
                var valuePlus1Result = await Add(passport, value, 1);
                var valuePlus1 = valuePlus1Result.Response;
                Passport.Scope.Record(() => valuePlus1);

                var valuePlus100Result = await Add100(passport, valuePlus1);
                var valuePlus100 = valuePlus100Result.Response;
                Passport.Scope.Record(() => valuePlus100);

                return true.GenerateStandardResponse();
            });
        }

        [LogParametersToPassport]
        public async Task<StandardResponse<int>> Add100(IPassport passport, int value)
        {
            return await this.ScopeAsync(async () => await Add(passport, value, 100));
        }

        [LogParametersToPassport]
        public async Task<StandardResponse<int>> Add(IPassport passport, int value, int increment)
        {
            return await this.ScopeAsync(async () =>
            {
                var result = value + increment;

                Passport.Scope.Record(() => result, "Added", record: true);

                await Task.Delay(1000);

                return result.GenerateStandardResponse();
            });
        }
    }
}
