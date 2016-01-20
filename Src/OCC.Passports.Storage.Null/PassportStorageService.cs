using OCC.Passports.Common.Contracts.Services;

namespace OCC.Passports.Storage.Null
{
    public class PassportStorageService : IPassportStorageService
    {
        public void Store(dynamic context)
        {
        }

        public void Flush()
        {
        }
    }
}
