namespace StatusNamaa;

internal interface IListenerService
{
    void RecordObservableInstruments();
    double? GetValue(string metricName);
}