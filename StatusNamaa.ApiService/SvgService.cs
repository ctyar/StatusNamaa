using System.Text;

namespace StatusNamaa.ApiService;

internal sealed class SvgService
{
    private static readonly List<Tuple<int, string>> Colors = new()
    {
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
    };

    public string GetSvg()
    {
        var svgDoc = new StringBuilder();

        AddHeader(svgDoc);

        AddMetricNames(svgDoc);

        AddMetricBars(svgDoc, [100, 70, 0, 19]);

        AddMetricValues(svgDoc, new List<Tuple<int, string>>
        {
            new(100, "100%"),
            new(70, "70%"),
            new(0, "0"),
            new(19, "19")
        });

        AddFooter(svgDoc);

        return svgDoc.ToString();
    }

    private static void AddMetricValues(StringBuilder svgDoc, List<Tuple<int, string>> values)
    {
        svgDoc.AppendLine("""<g font-size="18" font-weight="500" text-anchor="end">""");

        for (var i = 0; i < values.Count; i++)
        {
            var value = values[i];
            svgDoc.AppendLine($"<text x=\"365\" y=\"{50 + (i * 22)}\" fill=\"{GetColor(value.Item1)}\">{value.Item2}</text>");
        }

        svgDoc.AppendLine("</g>");
    }

    private static void AddMetricBars(StringBuilder svgDoc, List<int> values)
    {
        svgDoc.AppendLine("<g>");

        for (var i = 0; i < values.Count; i++)
        {
            AddMetricBar(svgDoc, values[i], i);
        }

        svgDoc.AppendLine("</g>");
    }

    private static void AddMetricBar(StringBuilder svgDoc, int value, int MetricIndex)
    {
        svgDoc.AppendLine("<g>");

        for (var i = 0; i < Colors.Count; i++)
        {
            var color = Colors[i].Item2;

            if (value < Colors[i].Item1)
            {
                break;
            }

            svgDoc.AppendLine($"<rect x=\"{161 + (i * 16)}\" y=\"{38 + (MetricIndex * 22)}\" width=\"13\" height=\"13\" rx=\"2\" fill=\"{color}\"/>");
        }

        svgDoc.AppendLine("</g>");
    }

    private static void AddMetricNames(StringBuilder svgDoc)
    {
        svgDoc.AppendLine("""
            <g font-size="13" font-weight="400">
              <text x="10" y="50" fill="#53b1fd">CPU</text>
              <text x="10" y="72" fill="#53b1fd">Memory</text>
              <text x="10" y="94" fill="#53b1fd">ThreadPool Queue Length</text>
              <text x="10" y="116" fill="#53b1fd">Lock Contentions</text>
            </g>
            """);
    }

    private static void AddHeader(StringBuilder svgDoc)
    {
        svgDoc.AppendLine("""
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;">
            <text x="10" y="24" fill="#bfc9d1" font-size="18" font-weight="500">Status Namma</text>
            """);
    }

    private static void AddFooter(StringBuilder svgDoc)
    {
        svgDoc.AppendLine("</svg>");
    }

    private static string GetColor(int value)
    {
        return value switch
        {
            >= 90 => "#f30b0b",
            >= 80 => "#fc3e21",
            >= 70 => "#fc9832",
            >= 60 => "#f9c03c",
            >= 50 => "#f9d94b",
            >= 40 => "#f2e95b",
            >= 30 => "#e0f75f",
            >= 20 => "#caf85e",
            >= 10 => "#b9fa63",
            _ => "#b0fd6a"
        };
    }
}
