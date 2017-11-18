using EnsureThat;
using NuGet.Server.Core.Logging;
using Serilog.Events;

namespace LightNuGetServer.Internals
{
    internal class SerilogProxyLogger : ILogger
    {
        private readonly Serilog.ILogger _serilogLogger;

        public SerilogProxyLogger(Serilog.ILogger serilogLogger)
        {
            Ensure.Any.IsNotNull(serilogLogger, nameof(serilogLogger));

            _serilogLogger = serilogLogger;
        }

        public void Log(LogLevel level, string message, params object[] args)
            => _serilogLogger.Write(GetSerilogLevel(level), message, args);

        private static LogEventLevel GetSerilogLevel(LogLevel level)
        {
            var serilogLevel = LogEventLevel.Verbose;

            switch (level)
            {
                case LogLevel.Verbose:
                    serilogLevel = LogEventLevel.Verbose;
                    break;
                case LogLevel.Info:
                    serilogLevel = LogEventLevel.Information;
                    break;
                case LogLevel.Warning:
                    serilogLevel = LogEventLevel.Warning;
                    break;
                case LogLevel.Error:
                    serilogLevel = LogEventLevel.Error;
                    break;
            }

            return serilogLevel;
        }
    }
}