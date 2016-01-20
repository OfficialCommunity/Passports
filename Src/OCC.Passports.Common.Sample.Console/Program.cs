using System.Threading.Tasks;
using OCC.Passports.Common.Contracts.Services;
using OCC.Passports.Common.Domains;
using OCC.Passports.Common.Infrastructure;
using OCC.Passports.Storage.Logentries;

namespace OCC.Passports.Common.Sample.Console
{
    class Program
    {
        private static readonly IPassportStorageService Storage = new PassportStorageService();

        static void Main(string[] args)
        {
            PassportLevel.Current = PassportLevel.Trace;

            Task t = WithController();
            t.Wait();

            Storage.Flush();
        }

        public static async Task<StandardResponse<int>> WithController()
        {
            var controller = new Controller(new Passport(Storage));

            return await controller.Calculate("1", 10, "/", 2);

        }

        public static async Task<StandardResponse<int>> WithControllerUsingServicer()
        {
            var controllerUsingService = new ControllerUsingService(new Passport(Storage), new Service());

            return await controllerUsingService.Calculate("2", 10, "/", 2);
        }
    }
}
