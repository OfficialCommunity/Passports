using System;
using System.Linq;

namespace OCC.Passports.Common.Infrastructure
{
    [Serializable]
    public abstract class StandardResponse
    {
        public StandardError StandardError;

        public bool HasError()
        {
            return (StandardError != null
                        && (StandardError.Errors != null && StandardError.Errors.Any())
                   );
        }
    }

    [Serializable]
    public class StandardResponse<T> : StandardResponse
    {
        public T Response;

    }
}
