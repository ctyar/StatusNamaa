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

#if DEBUG
        metrics[0].Value = 100;
        metrics[1].Value = 80;
        metrics[2].Value = 25;
#endif
        AddBody(svgDoc, metrics);

        AddFooter(svgDoc);

        return svgDoc.ToString();
    }

    private static void AddHeader(StringBuilder svgDoc)
    {
        svgDoc.AppendLine("""
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="620px" height="280px" viewBox="0 0 620 280">
            <text x="1rem" y="4rem" fill="#bfc9d1" font-size="4rem" font-weight="500">Status Namaa</text>
            """);
    }

    private void AddBody(StringBuilder svgDoc, List<MetricDisplayItem> metrics)
    {
        svgDoc.AppendLine("""<g font-size="2rem" font-weight="400">""");

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
        svgDoc.AppendLine($"""<text x="1rem" y="{7 + rowIndex * 2}rem" fill="#53b1fd">{metric.Name}</text>""");
    }

    private static void AddMetricBars(StringBuilder svgDoc, MetricDisplayItem metric, int rowIndex)
    {
        svgDoc.AppendLine("<g>");

        for (var i = 0; i < Colors.Count; i++)
        {
            var color = Colors[i].Item2;

            if (metric.Value.CompareTo(Colors[i].Item1) < 0)
            {
                break;
            }

            svgDoc.AppendLine($"""<rect x="{18 + 1.5 * i}rem" y="{6 + rowIndex * 2}rem" width="1rem" height="1rem" rx="0.2rem" fill="{color}"/>""");
        }

        svgDoc.AppendLine("</g>");
    }

    private static void AddMetricValue(StringBuilder svgDoc, MetricDisplayItem metric, int rowIndex)
    {
        svgDoc.AppendLine($"""<text x="33rem" y="{7 + rowIndex * 2}rem" fill="{GetColor(metric.Value)}">""" +
            $"{string.Format(metric.Format, metric.Value)}</text>");
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