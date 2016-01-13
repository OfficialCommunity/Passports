using OCC.Passports.Common.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace OCC.Passports.Common.Extensions
{
    public static class StandardResponseExtensions
    {
        public static StandardResponse<T> GenerateStandardResponse<T>(this T self)
        {
            var standardError = new StandardError();
            var response = new StandardResponse<T>
            {
                StandardError = standardError,
                Response = self
            };

            return response;
        }

        public static StandardResponse<T> GenerateStandardError<T>(this T self, string error)
        {
            var standardError = new StandardError();

            if (!string.IsNullOrWhiteSpace(error))
            {
                standardError.Errors.Add(error);
            }

            var response = new StandardResponse<T>
            {
                StandardError = standardError,
                Response = self
            };

            return response;
        }

        public static StandardResponse<T> GenerateStandardError<T>(this T self, ICollection<string> errors)
        {
            var standardError = new StandardError();

            if (errors != null && errors.Any())
            {
                standardError.Errors.AddRange(errors);
            }

            var response = new StandardResponse<T>
            {
                StandardError = standardError,
                Response = self
            };

            return response;
        }
    }
}
