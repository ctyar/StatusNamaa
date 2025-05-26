using System.Diagnostics.Metrics;
using Microsoft.Extensions.Diagnostics.Metrics;

namespace StatusNamaa.ApiService;

public class MetricsListener : IMetricsListener
{
    public static readonly Dictionary<string, long> MetricValues = [];

    private static IObservableInstrumentsSource ObservableInstrumentsSource = null!;

    public string Name => "MyMetricListener";

    public MeasurementHandlers GetMeasurementHandlers()
    {
        return new MeasurementHandlers
        {
            IntHandler = (instrument, measurement, tags, state) =>
            {
                if (MetricValues.ContainsKey(instrument.Name))
                {
                    MetricValues[instrument.Name] += measurement;
                }
                else
                {
                    MetricValues[instrument.Name] = measurement;
                }
            },
            LongHandler = (instrument, measurement, tags, state) =>
            {
                if (MetricValues.ContainsKey(instrument.Name))
                {
                    MetricValues[instrument.Name] += measurement;
                }
                else
                {
                    MetricValues[instrument.Name] = measurement;
                }
            },
            DecimalHandler = (instrument, measurement, tags, state) =>
            {
                if (MetricValues.ContainsKey(instrument.Name))
                {
                    MetricValues[instrument.Name] += (long)measurement;
                }
                else
                {
                    MetricValues[instrument.Name] = (long)measurement;
                }
            },
            DoubleHandler = (instrument, measurement, tags, state) =>
            {
                if (MetricValues.ContainsKey(instrument.Name))
                {
                    MetricValues[instrument.Name] += (long)measurement;
                }
                else
                {
                    MetricValues[instrument.Name] = (long)measurement;
                }
            },
            FloatHandler = (instrument, measurement, tags, state) =>
            {
                if (MetricValues.ContainsKey(instrument.Name))
                {
                    MetricValues[instrument.Name] += (long)measurement;
                }
                else
                {
                    MetricValues[instrument.Name] = (long)measurement;
                }
            },
            ShortHandler = (instrument, measurement, tags, state) =>
            {
                if (MetricValues.ContainsKey(instrument.Name))
                {
                    MetricValues[instrument.Name] += measurement;
                }
                else
                {
                    MetricValues[instrument.Name] = measurement;
                }
            },
            ByteHandler = (instrument, measurement, tags, state) =>
            {
                if (MetricValues.ContainsKey(instrument.Name))
                {
                    MetricValues[instrument.Name] += (long)measurement;
                }
                else
                {
                    MetricValues[instrument.Name] = (long)measurement;
                }
            }
        };
    }

    public void Initialize(IObservableInstrumentsSource source)
    {
        ObservableInstrumentsSource = source;
    }

    public bool InstrumentPublished(Instrument instrument, out object? userState)
    {
        userState = null;
        return true;
    }

    public void MeasurementsCompleted(Instrument instrument, object? userState)
    {
    }

    public static void Refresh()
    {
        ObservableInstrumentsSource.RecordObservableInstruments();
    }
}
