using Xunit;

namespace Serilog.Reload.Tests
{
    public class ReloadableLoggerTests
    {
        [Fact]
        public void AFrozenLoggerYieldsSerilogLoggers()
        {
            var logger = new ReloadableLogger(_ => _);
            var contextual = logger.ForContext<ReloadableLoggerTests>();

            var nested = contextual.ForContext("test", "test");
            Assert.IsNotType<Core.Logger>(nested);

            logger.Freeze();

            nested = contextual.ForContext("test", "test");
            Assert.IsType<Core.Logger>(nested);
        }
    }
}
