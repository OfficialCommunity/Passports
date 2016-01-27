﻿using System;
using System.Threading.Tasks;
using Destructurama;
using OCC.Passports.Common.Contracts.Services;
using OCC.Passports.Common.Domains;
using OCC.Passports.Common.Extensions;
using OCC.Passports.Common.Infrastructure;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.IOFile;

namespace OCC.Passports.Common.Sample.Console
{
    class Program
    {
        private static IPassportStorageService Storage = null;

        static void Main(string[] args)
        {
            var rootLogger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                //.WriteTo.File("log.txt")
                //.WriteTo.Sink(new FileSink(@"log.json", new JsonFormatter(false, null, true), null))
                .Enrich.FromLogContext()
                .Destructure.UsingAttributes()
                .Destructure.JsonNetTypes()
                //.WriteTo.Loggly()
                .CreateLogger();
            
            Storage = new Storage.Serilog.PassportStorageService(rootLogger);

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
            var passport = new Passport(Storage);

            var sensorInput = new { Latitude = 25, Longitude = 134 };
            passport.Error("{Now} {@SensorInput} Hello, Serilog!", new object[]
            {
                DateTime.UtcNow,
                sensorInput
            });
            var controllerUsingService = new ControllerUsingService(passport, new Service());

            return await controllerUsingService.Calculate("2", 10, "/", 0);
        }
    }
}
