using System.Text;
using Amazon.Lambda.APIGatewayEvents;
using MhLabs.Metrics;
using MhLabs.SerilogExtensions;
using MhLabs.WebApi.Observability.Framework.Extensions;
using MhLabs.WebApi.Observability.Framework.Logging;

namespace MhLabs.WebApi.Observability.Framework.Metrics
{
    public class Performance
    {
        public static readonly IMetricClient _client = new MetricClient(Settings.Stack);

        public static void Register(APIGatewayProxyRequest request, APIGatewayProxyResponse response, double elapsed)
        {
            if (Settings.IsUndefinedStack) return;

            var route = request.Headers.GetValueOrDefault(MhLabs.SerilogExtensions.Constants.RouteTemplateHeader);
            if (string.IsNullOrWhiteSpace(route)) return;

            var sb = new StringBuilder();

            sb.Append("route:");
            sb.Append(route);

            sb.Replace(Settings.Stack, string.Empty);
            sb.Replace("{", string.Empty);
            sb.Replace("}", string.Empty);
            sb.Replace("_", string.Empty);

            sb.Append($" method:{request.HttpMethod}");
            sb.Append($" status_code:{response.StatusCode}");

            _client.Gauge("api.requests.elapsed", (int) elapsed, sb.ToString());
        }
    }
}