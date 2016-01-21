using Newtonsoft.Json;
using OCC.Passports.Common;
using OCC.Passports.Common.Contracts.Services;
using System;

namespace OCC.Passports.Storage.Console
{
    public class PassportStorageService : IPassportStorageService
    {
        public void Store(dynamic context)
        {
            string message = null;
            try
            {
                message = JsonConvert.SerializeObject(context,Formatting.Indented);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error serializing exception {0}", ex.Message));

                if (PassportSettings.Settings.ThrowOnError)
                {
                    throw;
                }
            }

            if (message == null) return;
            try
            {
                System.Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error Logging Exception to Logentries {0}", ex.Message));

                if (PassportSettings.Settings.ThrowOnError)
                {
                    throw;
                }
            }
        }

        public void Flush()
        {
        }
    }
}
