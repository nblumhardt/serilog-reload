using Serilog.Reload;

namespace Serilog
{
    public static class LoggerConfigurationExtensions
    {
        public static ReloadableLogger CreateReloadableLogger(this LoggerConfiguration loggerConfiguration)
        {
            return new ReloadableLogger(loggerConfiguration.CreateLogger());
        }
    }
}