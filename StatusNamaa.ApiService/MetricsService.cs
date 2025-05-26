using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;

namespace StatusNamaa.ApiService;

public class MetricsService
{
    private static readonly MeterListener MeterListener = new();
    private static readonly Dictionary<string, long> MetricValues = [];

    public static readonly List<Metric> ExportedMetrics = [];

    public static void Init()
    {
        MeterListener.InstrumentPublished = (instrument, listener) =>
        {
            Console.WriteLine(instrument.Name);
            listener.EnableMeasurementEvents(instrument);
        };

        MeterListener.SetMeasurementEventCallback<int>(OnMeasurementRecorded);
        MeterListener.SetMeasurementEventCallback<long>(OnMeasurementRecorded);

        MeterListener.Start();
    }

    private static void OnMeasurementRecorded(Instrument instrument, int measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        if (!MetricValues.TryAdd(instrument.Name, measurement))
        {
            MetricValues[instrument.Name] += measurement;
        }
    }

    private static void OnMeasurementRecorded(Instrument instrument, long measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        if (!MetricValues.TryAdd(instrument.Name, measurement))
        {
            MetricValues[instrument.Name] += measurement;
        }
    }

    public long? GetValue(string metricName)
    {
        if (MetricValues.TryGetValue(metricName, out var value))
        {
            return value;
        }

        var metric = ExportedMetrics.FirstOrDefault(i => i.Name == metricName);

        if (metric is null)
        {
            return null;
        }

        return GetValue(metric);
    }

    private static long GetValue(Metric metric)
    {
        long sum = 0;

        foreach (ref readonly var metricPoint in metric.GetMetricPoints())
        {
            if (metric.MetricType.IsSum())
            {
                sum += metricPoint.GetSumLong();
            }
            else
            {
                sum += metricPoint.GetGaugeLastValueLong();
                break;
            }
        }

        return sum;
    }
}
