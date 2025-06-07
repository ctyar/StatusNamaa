using System.Diagnostics.Metrics;

namespace ApiService;

internal sealed class ListenerService
{
    private readonly MeterListener _meterListener = new();
    private int _requestCount;

    public ListenerService()
    {
        _meterListener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Meter.Name == QueueLengthMetric.MetricName && instrument.Name == QueueLengthMetric.InstrumentName)
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };

        _meterListener.SetMeasurementEventCallback<int>(OnMeasurementRecorded);

        _meterListener.Start();
    }

    public Task<int> GetValue()
    {
        return Task.FromResult(_requestCount);
    }

    private void OnMeasurementRecorded(Instrument instrument, int measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        _requestCount += measurement;
    }
}