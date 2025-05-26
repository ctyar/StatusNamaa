using System.Diagnostics.Metrics;

namespace StatusNamaa.ApiService;

public class RequestCountMetric
{
    public static readonly string MetricName = "StatusNamma.ApiService";
    public static readonly string InstrumentName = "request.count";

    private readonly UpDownCounter<int> _requestCount;

    public RequestCountMetric(IMeterFactory meterFactory)
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