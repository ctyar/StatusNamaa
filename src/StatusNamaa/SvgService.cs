using System.Text;

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

    public string GetSvg(List<MetricDisplayItem> metrics)
    {
        var svgDoc = new StringBuilder();

        AddHeader(svgDoc, metrics);

#if DEBUG
        metrics[0].Value = 100;
        metrics[1].Value = 80.134;
        metrics[2].Value = 25;
#endif
        AddBody(svgDoc, metrics);

        AddFooter(svgDoc, metrics);

        return svgDoc.ToString();
    }

    private static void AddHeader(StringBuilder svgDoc, List<MetricDisplayItem> metrics)
    {
        var height = 36 + 10 + ((metrics.Count + 2) * 24) + 10;
        svgDoc.AppendLine($"""
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="480px" height="{height}px" viewBox="0 0 480 {height}">
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            """);
    }

    private static void AddBody(StringBuilder svgDoc, List<MetricDisplayItem> metrics)
    {
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
        svgDoc.AppendLine($"""<text x="10px" y="{84 + rowIndex * 24}px" fill="#53b1fd">{Truncate(metric.Name)}</text>""");
    }

    private static void AddMetricBars(StringBuilder svgDoc, MetricDisplayItem metric, int rowIndex)
    {
        svgDoc.AppendLine("<g>");

        for (var i = 0; i < Colors.Count; i++)
        {
            var color = Colors[i].Item2;

            if (metric.Value is null || metric.Value.Value.CompareTo(Colors[i].Item1) < 0)
            {
                break;
            }

            svgDoc.AppendLine($"""<rect x="{10 + (17 * 12) + 20 * i}px" y="{(60 + 24 - 16) + rowIndex * 24}px" width="16px" height="16px" rx="2px" fill="{color}"/>""");
        }

        svgDoc.AppendLine("</g>");
    }

    private static void AddMetricValue(StringBuilder svgDoc, MetricDisplayItem metric, int rowIndex)
    {
        svgDoc.AppendLine($"""
            <text x="{480 - 10}px" y="{84 + rowIndex * 24}px"
             fill="{GetColor(metric.Value)}" text-anchor="end"> 
            """ +
            $"{string.Format(metric.Format, metric.Value)}</text>");
    }

    private static void AddFooter(StringBuilder svgDoc, List<MetricDisplayItem> metrics)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var version = MetricService.GetVersion();

        svgDoc.AppendLine($"""
            <text x="10px" y="{84 + (metrics.Count) * 24}px" fill="#53b1fd" font-size="10px">Environment: 
                <tspan fill="{Colors[0].Item2}">{environment}</tspan>
                 Version: 
                <tspan fill="{Colors[0].Item2}">{version}</tspan>
            </text>
            """);

        svgDoc.AppendLine("</svg>");
    }

    private static string GetColor(double? value)
    {
        if (value is null)
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