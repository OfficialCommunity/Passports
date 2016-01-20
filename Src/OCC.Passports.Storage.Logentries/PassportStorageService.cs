using System;
using Newtonsoft.Json;
using OCC.Passports.Common;
using OCC.Passports.Common.Contracts.Services;

namespace OCC.Passports.Storage.Logentries
{
    public class PassportStorageService : IPassportStorageService
    {
        public void Store(dynamic context)
        {
            string message = null;
            try
            {
                message = JsonConvert.SerializeObject(context);
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
                Logger.Current().AddLine(message);
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
            var numWaits = 3;
            while (!LogentriesCore.Net.AsyncLogger.AreAllQueuesEmpty(TimeSpan.FromSeconds(5)) && numWaits > 0)
                numWaits--;
            
        }
    }
}
