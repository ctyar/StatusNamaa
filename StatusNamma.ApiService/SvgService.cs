using System.Diagnostics.Metrics;
using System.Drawing;
using Svg;

namespace StatusNamma.ApiService;

public static class SvgService
{
    private static readonly Dictionary<string, long> Metrics = [];

    public static Stream GetSvg()
    {
        var svgDoc = new SvgDocument();
        var group = new SvgGroup();
        svgDoc.Children.Add(group);

        foreach (var metric in Metrics)
        {
            var quality = GetQuality(metric.Value);

            group.Children.Add(new SvgRectangle
            {
                Width = new SvgUnit(SvgUnitType.Em, 2),
                Height = new SvgUnit(SvgUnitType.Em, 4),
                Fill = GetColor(quality),
            });
        }

        var memoryStream = new MemoryStream();
        svgDoc.Write(memoryStream);
        memoryStream.Position = 0L;

        return memoryStream;
    }

    private static double? GetQuality(long? currentValue)
    {
        var bestValue = 0;
        var worstValue = 10;

        if (currentValue is null)
        {
            return null;
        }
        else if (currentValue <= bestValue)
        {
            return 0.0;
        }
        else if (currentValue >= worstValue)
        {
            return 1.0;
        }
        else
        {
            return (double)(currentValue - bestValue) / (worstValue - bestValue);
        }
    }

    private static SvgColourServer GetColor(double? quality)
    {
        if (quality is null)
        {
            return new SvgColourServer(Color.White);
        }

        var color = Blend(Color.Green, Color.Red, quality.Value);

        return new SvgColourServer(color);
    }

    public static Color Blend(Color color1, Color color2, double amount)
    {
        var r = (byte)(color1.R * (1 - amount) + color2.R * amount);
        var g = (byte)(color1.G * (1 - amount) + color2.G * amount);
        var b = (byte)(color1.B * (1 - amount) + color2.B * amount);

        return Color.FromArgb(r, g, b);
    }

    public static void RegisterListener(string[] names)
    {
        var meterListener = new MeterListener
        {
            InstrumentPublished = (instrument, listener) =>
            {
                Console.WriteLine(instrument.Name);
                if (names.Contains(instrument.Name))
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            }
        };

        meterListener.SetMeasurementEventCallback<int>(OnMeasurementRecorded);
        meterListener.SetMeasurementEventCallback<long>(OnMeasurementRecorded);

        meterListener.Start();
    }

    private static void OnMeasurementRecorded(Instrument instrument, int measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        Metrics.TryGetValue(instrument.Name, out var currentCount);
        Metrics[instrument.Name] = currentCount + measurement;
        Console.WriteLine("m:" + instrument.Name);
    }

    private static void OnMeasurementRecorded(Instrument instrument, long measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        Metrics.TryGetValue(instrument.Name, out var currentCount);
        Metrics[instrument.Name] = currentCount + measurement;
        Console.WriteLine("m:" + instrument.Name);
    }
}
