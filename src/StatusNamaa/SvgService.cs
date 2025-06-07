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

        AddHeader(svgDoc);

        AddMetricNames(svgDoc, metrics);

        AddMetricBars(svgDoc, metrics);

        AddMetricValues(svgDoc, metrics);

        AddFooter(svgDoc);

        return svgDoc.ToString();
    }

    private static void AddMetricValues(StringBuilder svgDoc, List<MetricDisplayItem> metrics)
    {
        svgDoc.AppendLine("""<g font-size="18" font-weight="500" text-anchor="end">""");

        for (var i = 0; i < metrics.Count; i++)
        {
            var metric = metrics[i];
            svgDoc.AppendLine($"<text x=\"331\" y=\"{50 + (i * 22)}\" fill=\"{GetColor(metric.Value)}\">{string.Format(metric.Format, metric.Value)}</text>");
        }

        svgDoc.AppendLine("</g>");
    }

    private static void AddMetricBars(StringBuilder svgDoc, List<MetricDisplayItem> metrics)
    {
        svgDoc.AppendLine("<g>");

        for (var i = 0; i < metrics.Count; i++)
        {
            AddMetricBar(svgDoc, metrics[i].Value, i);
        }

        svgDoc.AppendLine("</g>");
    }

    private static void AddMetricBar(StringBuilder svgDoc, double value, int MetricIndex)
    {
        svgDoc.AppendLine("<g>");

        for (var i = 0; i < Colors.Count; i++)
        {
            var color = Colors[i].Item2;

            if (value.CompareTo(Colors[i].Item1) < 0)
            {
                break;
            }

            svgDoc.AppendLine($"<rect x=\"{123 + (i * 16)}\" y=\"{38 + (MetricIndex * 22)}\" width=\"13\" height=\"13\" rx=\"2\" fill=\"{color}\"/>");
        }

        svgDoc.AppendLine("</g>");
    }

    private static void AddMetricNames(StringBuilder svgDoc, List<MetricDisplayItem> metrics)
    {
        svgDoc.AppendLine("""<g font-size="13" font-weight="400">""");

        for (var i = 0; i < metrics.Count; i++)
        {
            svgDoc.AppendLine($"""<text x="10" y="{50 + i * 22}" fill="#53b1fd">{metrics[i].Name}</text>""");
        }

        svgDoc.AppendLine("</g>");
    }

    private static void AddHeader(StringBuilder svgDoc)
    {
        svgDoc.AppendLine("""
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;">
            <text x="10" y="24" fill="#bfc9d1" font-size="18" font-weight="500">Status Namaa</text>
            """);
    }

    private static void AddFooter(StringBuilder svgDoc)
    {
        svgDoc.AppendLine("</svg>");
    }

    private static string GetColor(double value)
    {
        foreach (var color in Colors)
        {
            if (value <= color.Item1)
            {
                return color.Item2;
            }
        }

        return Colors.Last().Item2;
    }
}
