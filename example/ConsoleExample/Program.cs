using System;
using Serilog;
using Serilog.Events;
using Serilog.Reload;
using Serilog.Sinks.SystemConsole.Themes;

namespace ConsoleExample
{
    class Program
    {
        static void Main()
        {
            var logger = new ReloadableLogger(cfg => cfg.WriteTo.Console());

            Log.Logger = logger;

            Log.Information("Starting up");

            var programLogger = Log.ForContext<Program>();

            try
            {
                programLogger.Information("Hello, world!");

                logger.Reload(cfg => cfg
                    .MinimumLevel.Override("ConsoleExample", LogEventLevel.Error)
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code));

                // Level override now excludes this...
                programLogger.Information("Hello, again... maybe?");

                Log.Logger = logger.Freeze();
                
                throw new InvalidOperationException("That's all, folks.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Unhandled exception");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
