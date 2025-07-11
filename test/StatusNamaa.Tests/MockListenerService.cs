namespace StatusNamaa.Tests;

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