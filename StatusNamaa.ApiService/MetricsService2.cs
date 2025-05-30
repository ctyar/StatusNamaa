using System.Diagnostics.Metrics;

namespace StatusNamaa.ApiService;

internal sealed class MetricsService2
{
    private readonly MeterListener _meterListener = new();
    private readonly Dictionary<string, long> _metricValues = [];

    public MetricsService2()
    {
        _meterListener.InstrumentPublished = (instrument, listener) =>
        {
            Console.WriteLine(instrument.Name);
            listener.EnableMeasurementEvents(instrument);
        };

        _meterListener.SetMeasurementEventCallback<int>(OnMeasurementRecorded);
        _meterListener.SetMeasurementEventCallback<long>(OnMeasurementRecorded);

        _meterListener.Start();
    }

    public IEnumerable<long?> GetValues(string[] instrumentNames)
    {
        //_meterListener.RecordObservableInstruments();

        foreach (var instrumentName in instrumentNames)
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

        return null;
    }

    private void OnMeasurementRecorded(Instrument instrument, int measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        Console.WriteLine(instrument.Name);
        if (!_metricValues.TryAdd(instrument.Name, measurement))
        {
            _metricValues[instrument.Name] += measurement;
        }
    }

    private void OnMeasurementRecorded(Instrument instrument, long measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        Console.WriteLine(instrument.Name);
        if (!_metricValues.TryAdd(instrument.Name, measurement))
        {
            _metricValues[instrument.Name] += measurement;
        }
    }
}
