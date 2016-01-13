using LogentriesCore.Net;

namespace OCC.Passports.Storage.Logentries
{
    public class Logger
    {
        private static readonly Logger Instance = new Logger();

        private readonly AsyncLogger _writer;

        private Logger()
        {
            _writer = new AsyncLogger();
            _writer.setUseSsl(true);
            _writer.setUseHttpPut(false);
        }

        public static AsyncLogger Current()
        {
            return Instance._writer;
        }
    }
}
