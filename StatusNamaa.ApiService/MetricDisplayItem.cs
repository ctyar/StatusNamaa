namespace StatusNamaa.ApiService;

internal sealed class MetricDisplayItem
{
    public string Name { get; set; } = string.Empty;
    public long Value { get; set; }
    public string DisplayValue { get; set; } = string.Empty;

    public MetricDisplayItem(string name, long value, string displayValue)
    {
        Name = name;
        Value = value;
        DisplayValue = displayValue;
    }
}
