using Microsoft.Extensions.Options;

namespace StatusNamaa;

internal sealed class MetricDisplayService
{
    private readonly StatusNamaaOptions _options;
    private readonly IServiceProvider _serviceProvider;

    public MetricDisplayService(IOptions<StatusNamaaOptions> options, IServiceProvider serviceProvider)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
    }

    public async Task<List<MetricDisplayItem>> GetMetrics()
    {
        var metrics = new List<MetricDisplayItem>();

        foreach (var metric in _options.Metrics)
        {
            var value = await metric.Handler.Invoke(_serviceProvider);

            metrics.Add(new MetricDisplayItem(metric.Name, value, metric.Format));
        }

        return metrics;
    }
}