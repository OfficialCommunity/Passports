using Serilog;

namespace OCC.Passports.Storage.Serilog.Extensions
{
    public static class LoggingExtensions
    {
        public static ILogger With(this ILogger logger, string propertyName, object value)
        {
            return logger.ForContext(propertyName, value,true);
        }
    }
}
