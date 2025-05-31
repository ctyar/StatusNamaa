using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;

namespace StatusNamaa.ApiService;

internal sealed class ListenerService
{
    public readonly List<Metric> _exportedMetrics;
    private readonly string[] _instrumentNames;

    private readonly MeterListener _meterListener = new();
    private readonly Dictionary<string, int> _metricValues = [];
    private readonly Dictionary<string, long> _metricValuesLong = [];
    private readonly Dictionary<string, double> _metricValuesDouble = [];

    public ListenerService(List<Metric> exportedMetrics, string[] instrumentNames)
    {
        _exportedMetrics = exportedMetrics;
        _instrumentNames = instrumentNames;

        _meterListener.InstrumentPublished = (instrument, listener) =>
        {
            if (_instrumentNames.Contains(instrument.Name))
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };

        _meterListener.SetMeasurementEventCallback<int>(OnMeasurementRecorded);
        _meterListener.SetMeasurementEventCallback<long>(OnMeasurementRecorded);
        _meterListener.SetMeasurementEventCallback<double>(OnMeasurementRecorded);

        _meterListener.Start();
    }

    public IEnumerable<long?> GetValues()
    {
        _meterListener.RecordObservableInstruments();

        foreach (var instrumentName in _instrumentNames)
        {
            yield return GetValue(instrumentName);
        }
    }

    private long? GetValue(string metricName)
    {
        if (_metricValues.TryGetValue(metricName, out var value))
        {
            return value;
        }

        if (_metricValuesLong.TryGetValue(metricName, out var valueLong))
        {
            return valueLong;
        }

        if (_metricValuesDouble.TryGetValue(metricName, out var valueDouble))
        {
            return (long)valueDouble;
        }

        var metric = _exportedMetrics.FirstOrDefault(i => i.Name == metricName);

        if (metric is null)
        {
            return null;
        }

        return GetValue(metric);
    }

    private void OnMeasurementRecorded(Instrument instrument, int measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        if (!_metricValues.TryAdd(instrument.Name, measurement))
        {
            _metricValues[instrument.Name] += measurement;
        }
    }

    private void OnMeasurementRecorded(Instrument instrument, long measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        if (!_metricValuesLong.TryAdd(instrument.Name, measurement))
        {
            _metricValuesLong[instrument.Name] += measurement;
        }
    }

    private void OnMeasurementRecorded(Instrument instrument, double measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        if (!_metricValuesDouble.TryAdd(instrument.Name, measurement))
        {
            _metricValuesDouble[instrument.Name] += measurement;
        }
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
