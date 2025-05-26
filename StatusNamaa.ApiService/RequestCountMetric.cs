using System.Diagnostics.Metrics;

namespace StatusNamaa.ApiService;

public class RequestCountMetric
{
    public static readonly string Name = "StatusNamma.ApiService";

    private readonly UpDownCounter<int> _requestCount;

    public RequestCountMetric(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(Name);

        _requestCount = meter.CreateUpDownCounter<int>("request.count");
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