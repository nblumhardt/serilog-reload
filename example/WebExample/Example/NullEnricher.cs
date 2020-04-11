using Serilog.Core;
using Serilog.Events;

namespace WebExample.Example
{
    // This exists for now only as a workaround
    class NullEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
        }
    }
}
