using Microsoft.Extensions.Options;

namespace StatusNamaa;

internal sealed class MetricDisplayService
{
    private readonly StatusNamaaOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ListenerService _listenerService;

    public MetricDisplayService(IOptions<StatusNamaaOptions> options, IServiceProvider serviceProvider,
        ListenerService listenerService)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _listenerService = listenerService;
    }

    public async Task<List<MetricDisplayItem>> GetMetrics()
    {
        var metrics = new List<MetricDisplayItem>();
        var isRecorded = false;

        foreach (var metric in _options.Metrics)
        {
            double? value;

            if (metric.Selector is not null)
            {
                value = await metric.Selector.Invoke(_serviceProvider);
            }
            else
            {
                if (!isRecorded)
                {
                    _listenerService.RecordObservableInstruments();
                    isRecorded = true;
                }

                value = _listenerService.GetValue(metric.Name);
            }

            metrics.Add(new MetricDisplayItem(metric.Name, value, metric.Format));
        }

        return metrics;
    }
}