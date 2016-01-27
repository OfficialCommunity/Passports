using System.Collections.Generic;
using OCC.Passports.Common.Infrastructure;

namespace OCC.Passports.Common.Contracts.Services
{
    public interface IPassportStorageService
    {
        void Store(MessageContext messageContext, IDictionary<string, object> snapshot);
        void Flush();
    }
}
