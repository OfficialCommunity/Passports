using System;
using System.Runtime.Remoting.Messaging;
using Destructurama;
using Microsoft.WindowsAzure.Storage;
using OCC.Passports.Common.Contracts.Services;
using OCC.Passports.Common.Domains;
using OCC.Passports.Common.Extensions;
using OCC.Passports.Common.Infrastructure;
using OCC.Passports.Storage.Serilog.Sinks.Passport;
using Serilog;
using Serilog.Sinks.Datadog;
using System.Threading.Tasks;

namespace OCC.Passports.Common.Sample.Console
{
    class Program
    {
        private static IPassportStorageService _storage = null;

        static void Main(string[] args)
        {
            //var config = new DatadogConfiguration()
            //    //.WithWithStatsdServer("127.0.0.1", 8125)
            //    .WithHostname("passport")
            //    ;

            var storage = CloudStorageAccount.DevelopmentStorageAccount;

            var rootLogger = new LoggerConfiguration()
                //.ReadFrom.AppSettings()
                //.WriteTo.File("log.txt")
                //.WriteTo.Sink(new FileSink(@"log.json", new JsonFormatter(false, null, true), null))
                //.Enrich.FromLogContext()
                //.Destructure.UsingAttributes()
                .Destructure.JsonNetTypes()
                .WriteTo.AzureTableStorageWithProperties(storage)
                .MinimumLevel.Debug()
                //.WriteTo.Loggly()
                .CreateLogger();
            
            var passportLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Sink(new PassportSink(rootLogger))
                .Destructure.JsonNetTypes()
                .CreateLogger();

            _storage = new Storage.Serilog.PassportStorageService(passportLogger);

            PassportLevel.Current = PassportLevel.Debug;

            //var result = WithControllerUsingService().Result;
            var result = WithControllerAsyncTest().Result;

            System.Console.WriteLine("press any key to continue");
            System.Console.ReadKey();

            _storage.Flush();
        }

        public static async Task<StandardResponse<int>> WithController()
        {
            var controller = new Controller(new Passport(_storage));
            return await controller.Calculate("1", 10, "/", 0);

        }

        public static async Task<StandardResponse<int>> WithControllerUsingService()
        {
            var passport = new Passport(_storage);

            var controllerUsingService = new ControllerUsingService(passport, new Service());

            //controllerUsingService.Calculate("2", 10, "/", 2);
            return await controllerUsingService.Calculate("2", 10, "/", 5);
        }

        public static async Task<StandardResponse<bool>> WithControllerAsyncTest()
        {
            var passport = new Passport(_storage);

            var controller = new Controller(passport);

            await controller.AdditionTest(Guid.NewGuid().ToString("N"));
            return true.GenerateStandardResponse();
        }
    }
}
