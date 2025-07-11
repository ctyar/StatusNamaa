using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace StatusNamaa;

internal sealed class SvgService
{
    private static readonly List<Tuple<int, string>> Colors =
    [
        new(0, "#b0fd6a"),
        new(10, "#b9fa63"),
        new(20, "#caf85e"),
        new(30, "#e0f75f"),
        new(40, "#f2e95b"),
        new(50, "#f9d94b"),
        new(60, "#f9c03c"),
        new(70, "#fc9832"),
        new(80, "#fc3e21"),
        new(90, "#f30b0b"),
    ];

    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ICustomMetricService _customMetricService;

    public SvgService(IWebHostEnvironment webHostEnvironment, ICustomMetricService customMetricService)
    {
        _webHostEnvironment = webHostEnvironment;
        _customMetricService = customMetricService;
    }

    public string GetSvg(List<MetricDisplayItem> metrics)
    {
        var svgDoc = new StringBuilder();

        AddHeader(svgDoc, metrics);

        AddBody(svgDoc, metrics);

        AddFooter(svgDoc, metrics);

        return svgDoc.ToString();
    }

    private static void AddHeader(StringBuilder svgDoc, List<MetricDisplayItem> metrics)
    {
        var height = 36 + 10 + ((metrics.Count + 2) * 24) + 10;
        svgDoc.AppendLine($"""
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="{height}px" viewBox="0 0 470 {height}">
            <clipPath id="clip1">
                <rect x="10px" y="10px" width="180px" height="{84 + (metrics.Count) * 24}px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            """);
    }

    private static void AddBody(StringBuilder svgDoc, List<MetricDisplayItem> metrics)
    {
        if (metrics.Count == 0)
        {
            return;
        }

        svgDoc.AppendLine("""<g font-size="24px" font-weight="400">""");

        for (var i = 0; i < metrics.Count; i++)
        {
            svgDoc.AppendLine("""<g>""");

            AddMetricName(svgDoc, metrics[i], i);

            AddMetricBars(svgDoc, metrics[i], i);

            AddMetricValue(svgDoc, metrics[i], i);

            svgDoc.AppendLine("</g>");
        }

        svgDoc.AppendLine("</g>");
    }

    private static void AddMetricName(StringBuilder svgDoc, MetricDisplayItem metric, int rowIndex)
    {
        svgDoc.AppendLine($"""<g clip-path="url(#clip1)"><text x="10px" y="{84 + rowIndex * 24}px" fill="#53b1fd">{Truncate(metric.Name)}</text></g>""");
    }

    private static void AddMetricBars(StringBuilder svgDoc, MetricDisplayItem metric, int rowIndex)
    {
        if (metric.Type != StatusNamaaValueType.Percentage)
        {
            return;
        }

        svgDoc.AppendLine("<g>");

        for (var i = 0; i < Colors.Count; i++)
        {
            var color = Colors[i].Item2;

            if (metric.Value is null || metric.Value.Value.CompareTo(Colors[i].Item1) < 0)
            {
                break;
            }

            svgDoc.AppendLine($"""<rect x="{10 + (190) + 20 * i}px" y="{(60 + 24 - 16) + rowIndex * 24}px" width="16px" height="16px" rx="2px" fill="{color}"/>""");
        }

        svgDoc.AppendLine("</g>");
    }

    private static void AddMetricValue(StringBuilder svgDoc, MetricDisplayItem metric, int rowIndex)
    {
        svgDoc.AppendLine($"""
            <text x="{470 - 10}px" y="{84 + rowIndex * 24}px" fill="{GetColor(metric.Value, metric.Type)}" text-anchor="end">{metric.DisplayValue}</text>
            """);
    }

    private void AddFooter(StringBuilder svgDoc, List<MetricDisplayItem> metrics)
    {
        var environment = _webHostEnvironment.EnvironmentName;
        var version = _customMetricService.GetVersion();

        svgDoc.AppendLine($"""
            <text x="10px" y="{84 + (metrics.Count) * 24}px" fill="#53b1fd" font-size="10px">Environment: <tspan fill="{Colors[0].Item2}">{environment}</tspan>  Version: <tspan fill="{Colors[0].Item2}">{version}</tspan></text>
            """);

        svgDoc.Append("</svg>");
    }

    private static string GetColor(double? value, StatusNamaaValueType type)
    {
        if (value is null)
        {
            return Colors[0].Item2;
        }

        if (type != StatusNamaaValueType.Percentage)
        {
            return Colors[0].Item2;
        }

        foreach (var color in Colors)
        {
            if (value <= color.Item1)
            {
                return color.Item2;
            }
        }

        return Colors.Last().Item2;
    }

    public static string Truncate(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Length <= 16 ? value : value[..16];
    }
}