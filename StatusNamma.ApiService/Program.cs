using System.Diagnostics.Metrics;
using System.Drawing;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Svg;

namespace StatusNamma.ApiService;

public class Program
{
    private static readonly Dictionary<string, int> Values = [];

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults(RequestCountMetric.Name);

        // Add services to the container.
        builder.Services.AddProblemDetails();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerUI();

        builder.Services.AddSingleton<RequestCountMetric>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapSwaggerUI();
        }

        string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

        app.MapGet("/produce", ([FromServices] RequestCountMetric requestCountMetric) =>
        {
            requestCountMetric.Produce();

            return "done";
        });
        app.MapGet("/consume", ([FromServices] RequestCountMetric requestCountMetric) =>
        {
            requestCountMetric.Consume();

            return "done";
        });

        app.MapGet("/statusnamma", () =>
        {
            var quality = GetQuality("request.count");

            var svgDoc = new SvgDocument();
            var group = new SvgGroup();
            svgDoc.Children.Add(group);

            group.Children.Add(new SvgRectangle
            {
                Width = new SvgUnit(SvgUnitType.Em, 5),
                Height = new SvgUnit(SvgUnitType.Em, 5),
                Fill = GetColor(quality),
            });

            var content = svgDoc.GetXML();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            return Results.File(stream, "image/svg+xml");
        });

        app.MapDefaultEndpoints();

        SetListener();

        app.Run();
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

    private static double? GetQuality(string name)
    {
        var bestValue = 0;
        var worstValue = 10;

        if (!Values.TryGetValue(name, out var currentValue))
        {
            return null;
        }

        if (currentValue <= bestValue)
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

    public static Color Blend(Color color1, Color color2, double amount)
    {
        var r = (byte)(color1.R * (1 - amount) + color2.R * amount);
        var g = (byte)(color1.G * (1 - amount) + color2.G * amount);
        var b = (byte)(color1.B * (1 - amount) + color2.B * amount);

        return Color.FromArgb(r, g, b);
    }

    private static void SetListener()
    {
        var meterListener = new MeterListener
        {
            InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name == RequestCountMetric.Name)
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            }
        };

        meterListener.SetMeasurementEventCallback<int>(OnMeasurementRecorded);

        meterListener.Start();
    }

    private static void OnMeasurementRecorded(Instrument instrument, int measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        Values.TryGetValue(instrument.Name, out var currentCount);
        Values[instrument.Name] = currentCount + measurement;

        Console.WriteLine($"{instrument.Name} recorded measurement {measurement}");
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class RequestCountMetric
{
    public static readonly string Name = "StatusNamma.ApiService";

    private readonly UpDownCounter<int> _requestCount;

    public RequestCountMetric(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(Name);

        _requestCount = meter.CreateUpDownCounter<int>("request.count");
    }

    public void Produce()
    {
        _requestCount.Add(1);
    }

    public void Consume()
    {
        _requestCount.Add(-1);
    }
}