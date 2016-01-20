﻿using System.Threading.Tasks;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Extensions;
using OCC.Passports.Common.Infrastructure;
using OCC.Passports.Common.Sample.Console.Contracts;

namespace OCC.Passports.Common.Sample.Console
{
    class Service : IService
    {
        public async Task<StandardResponse<int>> Calculate(IPassport passport, int lhs, string @operator, int rhs)
        {
            return await passport.ScopeAsync(async () =>
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
                        passport.Scope.Record(() => result, "Operation failed");
                        passport.Error(invalideOperatorMessage);
                        return result.GenerateStandardError(invalideOperatorMessage);
                }
                passport.Scope.Record(() => result, "Operation succeeded");
                passport.Trace("End of request");
                return result.GenerateStandardResponse();
            });
        }

    }
}
