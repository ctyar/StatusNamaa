using System.Diagnostics.Metrics;

namespace ApiService;

public class QueueLengthMetric
{
    public static readonly string MetricName = "ApiService";
    public static readonly string InstrumentName = "queue.length";

    private readonly UpDownCounter<int> _requestCount;

    public QueueLengthMetric(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MetricName);

        _requestCount = meter.CreateUpDownCounter<int>(InstrumentName);
    }

    public void Produce()
    {
        _requestCount.Add(1);
    }

    public void Consume()
    {
        _requestCount.Add(-1);
    }
}