namespace StatusNamaa;

internal sealed class MetricDisplayItem
{
    public string Name { get; set; }
    public StatusNamaaValueType Type { get; set; }
    public double? Value { get; set; }
    public string? DisplayValue { get; set; }

    public MetricDisplayItem(string name, StatusNamaaValueType type, double? value, string? displayValue)
    {
        Name = name;
        Type = type;
        Value = value;
        DisplayValue = displayValue;
    }
}