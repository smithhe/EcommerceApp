using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Parsing;
using Serilog.Sinks.Grafana.Loki;

namespace Ecommerce.Api
{
    [SuppressMessage(
        "ReSharper",
        "PossibleMultipleEnumeration",
        Justification = "Reviewed")]
    public class EcommerceLogTextFormatter : LokiJsonTextFormatter
    {
        public override void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            output.Write("{\"Message\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Render(logEvent.Properties), output);

            output.Write(",\"MessageTemplate\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Text, output);

            IEnumerable<PropertyToken> tokensWithFormat = logEvent.MessageTemplate.Tokens
                .OfType<PropertyToken>()
                .Where(pt => pt.Format != null);

            // Better not to allocate an array in the 99.9% of cases where this is false
            if (tokensWithFormat.Any())
            {
                output.Write(",\"Renderings\":[");
                string delimiter = string.Empty;
                foreach (PropertyToken r in tokensWithFormat)
                {
                    output.Write(delimiter);
                    delimiter = ",";
                    StringWriter space = new StringWriter();
                    r.Render(logEvent.Properties, space);
                    JsonValueFormatter.WriteQuotedJsonString(space.ToString(), output);
                }

                output.Write(']');
            }

            if (logEvent.Exception != null)
            {
                output.Write(",\"Exception\":");
                SerializeException(
                    output,
                    logEvent.Exception,
                    1);
            }

            //Update the property to information if it is info to fix a bug in Grafana Loki
            Dictionary<string, LogEventPropertyValue> modifiableProperties = new Dictionary<string, LogEventPropertyValue>(logEvent.Properties);
            if (modifiableProperties.ContainsKey("level"))
            {
                if (modifiableProperties["level"] is ScalarValue scalarValue)
                {
                    if (scalarValue.Value?.ToString()?.Equals("info", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        modifiableProperties["level"] = new ScalarValue("information");
                    }
                }
            }
            
            if (modifiableProperties.ContainsKey("SourceContext"))
            {
                modifiableProperties.Remove("SourceContext");
            }
            modifiableProperties.Add("source_context", new ScalarValue(logEvent.Properties["SourceContext"].ToString()));
            
            foreach (var (key, value) in modifiableProperties)
            {
                string name = GetSanitizedPropertyName(key);
                output.Write(',');
                JsonValueFormatter.WriteQuotedJsonString(name, output);
                output.Write(':');
                ValueFormatter.Format(value, output);
            }

            output.Write('}');
        }
    }
}