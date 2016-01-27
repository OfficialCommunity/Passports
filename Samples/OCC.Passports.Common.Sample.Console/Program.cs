﻿using System.Threading.Tasks;
using OCC.Passports.Common.Contracts.Services;
using OCC.Passports.Common.Domains;
using OCC.Passports.Common.Infrastructure;

namespace OCC.Passports.Common.Sample.Console
{
    class Program
    {
        private static readonly IPassportStorageService Storage = new OCC.Passports.Storage.Console.PassportStorageService();

        static void Main(string[] args)
        {
            PassportLevel.Current = PassportLevel.Debug;

            var result = WithControllerUsingService().Result;

            System.Console.WriteLine("press any key to continue");
            System.Console.ReadKey();

            Storage.Flush();
        }

        public static async Task<StandardResponse<int>> WithController()
        {
            var controller = new Controller(new Passport(Storage));

            return await controller.Calculate("1", 10, "/", 0);

        }

        public static async Task<StandardResponse<int>> WithControllerUsingService()
        {
            var controllerUsingService = new ControllerUsingService(new Passport(Storage), new Service());

            return await controllerUsingService.Calculate("2", 10, "/", 0);
        }
    }
}
