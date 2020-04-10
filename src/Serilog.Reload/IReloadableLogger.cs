namespace Serilog.Reload
{
    interface IReloadableLogger
    {
        ILogger ReloadLogger();
    }
}