using System;
using System.Globalization;
using OCC.Passports.Common.Contracts.Infrastructure;

namespace OCC.Passports.Common.Infrastructure.Contexts
{
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

        public string MachineName { get; private set; }
        public double UtcOffset { get; private set; }
        public string Locale { get; private set; }
    }
}
