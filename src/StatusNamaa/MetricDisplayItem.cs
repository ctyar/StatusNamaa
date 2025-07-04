namespace StatusNamaa;

internal sealed class MetricDisplayItem
{
    public string Name { get; set; }
    public double? Value { get; set; }
    public string? DisplayValue { get; set; }

    public MetricDisplayItem(string name, double? value, string? displayValue)
    {
        Name = name;
        Value = value;
        DisplayValue = displayValue;
    }
}