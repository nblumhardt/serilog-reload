# Serilog.Reload [![Build status](https://ci.appveyor.com/api/projects/status/xrdx9mhx8ttayfbh/branch/master?svg=true)](https://ci.appveyor.com/project/NicholasBlumhardt/serilog-reload/branch/master) [![NuGet Version](https://img.shields.io/nuget/vpre/Serilog.Reload)](https://www.nuget.org/packages/Serilog.Reload)

A reloadable Serilog wrapper, under active development.

This package provides `ReloadableLogger`, a Serilog `ILogger` that can be updated with new configuration after creation.

`ReloadableLogger` is built around the assumption that reloads will be infrequent (usually just once or twice during 
start-up of an application), and that the logger will eventually be "frozen" into a form that  the application will use 
for the remainder of its lifetime.

To support this, `ReloadableLogger` starts in a mutable state, which requires internal synchronization using `lock` when
 `ILogger` methods are called:

```csharp
var logger = new ReloadableLogger(cfg => cfg.WriteTo.Console());
Log.Logger = logger;

// Writes to the console; this is slightly more expensive than a regular
// Serilog write, since the entire operation must take place within a critical section.
logger.Information("Hello, world!");
```

Calling `ReloadableLogger.Reload(..)` in this state will tear down the current logging pipeline behind-the-scenes and 
reconfigure it through the provided callback:

```csharp
logger.Reload(cfg => cfg
	.WriteTo.Console()
	.WriteTo.File("./log.txt"));

// Writes to the console and `log.txt` file.
logger.Information("Hello, again");
```

When the logger is reloaded, any contextual loggers created from it will also be reloaded and the new configuration 
applied to them.

Finally, when logging is fully configured, the logger should be frozen into an immutable state:

```csharp
Log.Logger = logger.Freeze();
```

`ReloadableLogger.Freeze()` causes the reloadable logger and all loggers derived from it to become immutable, and able 
to act as a thin wrapper over Serilog, removing the synchronization overhead that is necessary in the mutable state. 
Further contextual loggers created from these will be raw Serilog ones, with no additional overhead.

`Freeze()` returns the frozen Serilog logger, removing the need to call through the wrapper entirely for subsequent 
logging calls.
