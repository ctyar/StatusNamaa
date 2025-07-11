namespace StatusNamaa.Tests.Mocks;

internal sealed class MockListenerService : IListenerService
{
    public double? GetValue(string metricName)
    {
        return 5;
    }

    public void RecordObservableInstruments()
    {
    }
}