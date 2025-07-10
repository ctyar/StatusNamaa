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

            var displayName = metric.DisplayName ?? metric.Name;

            string? displayValue;
            if (metric.Formatter is not null)
            {
                displayValue = metric.Formatter(value);
            }
            else
            {
                displayValue = Format(value, metric.Type);
            }

            metrics.Add(new MetricDisplayItem(displayName, metric.Type, value, displayValue));
        }

        /*#if DEBUG
                metrics[0].Value = 100;
                metrics[0].DisplayValue = "100%";
                metrics[1].Value = 70;
                metrics[1].DisplayValue = "70%";
                metrics[2].Value = 25;
                metrics[2].DisplayValue = "25";
        #endif*/

        return metrics;
    }

    private static string? Format(double? value, StatusNamaaValueType type)
    {
        if (value is null)
        {
            return null;
        }

        return type switch
        {
            StatusNamaaValueType.Bytes => FormatBytes(value.Value),
            StatusNamaaValueType.Percentage => string.Format("{0:F0}%", value),
            _ => value.Value.ToString()
        };
    }

    private static string FormatBytes(double value)
    {
        if (value < 1024)
        {
            return $"{value:F0} B";
        }
        else if (value < 1024 * 1024)
        {
            return $"{value / 1024:F0} KB";
        }
        else if (value < 1024 * 1024 * 1024)
        {
            return $"{value / (1024 * 1024):F0} MB";
        }
        else
        {
            return $"{value / (1024 * 1024 * 1024):F0} GB";
        }
    }
}