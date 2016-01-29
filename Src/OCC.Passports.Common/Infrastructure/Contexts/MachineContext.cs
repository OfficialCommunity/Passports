using System;
using System.Globalization;
using Newtonsoft.Json;
using OCC.Passports.Common.Contracts.Infrastructure;

namespace OCC.Passports.Common.Infrastructure.Contexts
{
    [Serializable]
    public class MachineContext : IContext
    {
        public MachineContext()
        {
            try
            {
                MachineName = Environment.MachineName;
                var now = DateTime.Now;
                UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(now).TotalHours;
                Locale = CultureInfo.CurrentCulture.DisplayName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Error retrieving time and locale: {0}", ex.Message);
            }
            
        }

        [JsonProperty(Order = 1)]
        public string MachineName { get; private set; }
        [JsonProperty(Order = 2)]
        public string Locale { get; private set; }
        [JsonProperty(Order = 3)]
        public double UtcOffset { get; private set; }
    }
}
