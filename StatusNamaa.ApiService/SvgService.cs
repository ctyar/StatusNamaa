﻿using System.Drawing;
using Svg;

namespace StatusNamaa.ApiService;

internal sealed class SvgService
{
    private readonly ListenerService _metricsService;

    public SvgService(ListenerService metricsService)
    {
        _metricsService = metricsService;
    }

    public Stream GetSvg()
    {
        var svgDoc = new SvgDocument();
        var group = new SvgGroup();
        svgDoc.Children.Add(group);

        var x = 0;

        foreach (var metricValue in _metricsService.GetValues())
        {
            var color = GetColor(metricValue);

            group.Children.Add(new SvgRectangle
            {
                Width = new SvgUnit(SvgUnitType.Em, 2),
                Height = new SvgUnit(SvgUnitType.Em, 4),
                Fill = color,
                StrokeWidth = new SvgUnit(SvgUnitType.Em, 0.05f),
                Stroke = new SvgColourServer(Color.Black),
                X = new SvgUnit(SvgUnitType.Em, x),
            });

            x += 2;
        }

        var memoryStream = new MemoryStream();
        svgDoc.Write(memoryStream);
        memoryStream.Position = 0L;

        return memoryStream;
    }

    private static SvgColourServer GetColor(long? currentValue)
    {
        if (currentValue is null)
        {
            return new SvgColourServer(Color.Transparent);
        }

        var bestValue = 0;
        var middleValue = 5;
        var worstValue = 10;

        if (currentValue <= bestValue)
        {
            return new SvgColourServer(Color.Green);
        }
        else if (currentValue <= middleValue)
        {
            var percentage = GetPercentage(currentValue.Value, bestValue, middleValue);

            return Blend(Color.Green, Color.Yellow, percentage);
        }
        else if (currentValue <= worstValue)
        {
            var percentage = GetPercentage(currentValue.Value, bestValue, middleValue);

            return Blend(Color.Yellow, Color.Red, percentage);
        }
        else
        {
            return new SvgColourServer(Color.Red);
        }
    }

    private static SvgColourServer Blend(Color color1, Color color2, double amount)
    {
        var r = (byte)(color1.R * (1 - amount) + color2.R * amount);
        var g = (byte)(color1.G * (1 - amount) + color2.G * amount);
        var b = (byte)(color1.B * (1 - amount) + color2.B * amount);

        return new SvgColourServer(Color.FromArgb(r, g, b));
    }

    private static double GetPercentage(long currentValue, long minValue, long maxValue)
    {
        return (double)(currentValue - minValue) / (maxValue - minValue);
    }
}
