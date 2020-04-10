using Serilog.Core;
using Serilog.Events;

namespace Serilog.Reload
{
    class FixedPropertyEnricher : ILogEventEnricher
    {
        readonly LogEventProperty _property;

        public FixedPropertyEnricher(LogEventProperty property)
        {
            _property = property;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(_property);
        }
    }
}
