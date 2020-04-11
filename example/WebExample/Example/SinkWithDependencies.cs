using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using Serilog.Core;
using Serilog.Events;

namespace WebExample.Example
{
    public class SinkWithDependencies : ILogEventSink
    {
        readonly Lazy<HttpClient> _httpClient;

        public SinkWithDependencies(Lazy<IHttpClientFactory> httpClientFactory)
        {
            _httpClient = new Lazy<HttpClient>(() => httpClientFactory.Value.CreateClient("logging"));
        }

        public void Emit(LogEvent logEvent)
        {
            var payload = new Dictionary<string, object>
            {
                ["@t"] = logEvent.Timestamp.ToString("o"),
                ["@m"] = logEvent.RenderMessage(),
                ["Sink"] = nameof(SinkWithDependencies)
            };
            
            // Writes to a local https://datalust.co/seq instance
            _httpClient.Value.PostAsJsonAsync("http://localhost:5341/api/events/raw?clef", payload).Wait();
        }
    }
}

