using OCC.Passports.Common.Contracts.Services;
using OCC.Passports.Common.Infrastructure.Contexts;

namespace OCC.Passports.Storage.Null
{
    public class PassportStorageService : IPassportStorageService
    {
        public void Flush()
        {
        }

        public void Store(MessageContext messageContext, System.Collections.Generic.IDictionary<string, object> snapshot)
        {
        }
    }
}
