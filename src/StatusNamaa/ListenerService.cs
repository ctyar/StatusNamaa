using System.Diagnostics.Metrics;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;

namespace StatusNamaa;

internal sealed class ListenerService
{
    public static List<Metric> ExportedMetrics { get; set; } = [];

    private readonly MeterListener _meterListener = new();
    private readonly Dictionary<string, double> _metricValues = [];

    public ListenerService(IOptions<StatusNamaaOptions> options)
    {
        var instrumentNames = options.Value.Metrics.Select(m => m.Name).ToArray();

        _meterListener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrumentNames.Contains(instrument.Name))
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };

        _meterListener.SetMeasurementEventCallback<byte>(OnMeasurementRecorded);
        _meterListener.SetMeasurementEventCallback<int>(OnMeasurementRecorded);
        _meterListener.SetMeasurementEventCallback<float>(OnMeasurementRecorded);
        _meterListener.SetMeasurementEventCallback<double>(OnMeasurementRecorded);
        _meterListener.SetMeasurementEventCallback<decimal>(OnMeasurementRecorded);
        _meterListener.SetMeasurementEventCallback<short>(OnMeasurementRecorded);
        _meterListener.SetMeasurementEventCallback<long>(OnMeasurementRecorded);

        _meterListener.Start();
    }

    public void RecordObservableInstruments()
    {
        _meterListener.RecordObservableInstruments();
    }

    public double? GetValue(string metricName)
    {
        if (_metricValues.TryGetValue(metricName, out var value))
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

    private void OnMeasurementRecorded(Instrument instrument, double measurement)
    {
        if (!_metricValues.TryAdd(instrument.Name, measurement))
        {
            _metricValues[instrument.Name] += measurement;
        }
    }

    private void OnMeasurementRecorded(Instrument instrument, byte measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        OnMeasurementRecorded(instrument, measurement);
    }

    private void OnMeasurementRecorded(Instrument instrument, int measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        OnMeasurementRecorded(instrument, measurement);
    }

    private void OnMeasurementRecorded(Instrument instrument, float measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        OnMeasurementRecorded(instrument, measurement);
    }

    private void OnMeasurementRecorded(Instrument instrument, double measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        OnMeasurementRecorded(instrument, measurement);
    }

    private void OnMeasurementRecorded(Instrument instrument, decimal measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        OnMeasurementRecorded(instrument, (double)measurement);
    }

    private void OnMeasurementRecorded(Instrument instrument, short measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        OnMeasurementRecorded(instrument, measurement);
    }

    private void OnMeasurementRecorded(Instrument instrument, long measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        OnMeasurementRecorded(instrument, measurement);
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