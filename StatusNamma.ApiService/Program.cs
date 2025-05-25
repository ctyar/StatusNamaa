using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace StatusNamma.ApiService;

public class Program
{
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

        app.MapGet("/weatherforecast", ([FromServices] RequestCountMetric requestCountMetric) =>
        {
            requestCountMetric.RequestReceived();

            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast");

        app.MapDefaultEndpoints();

        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class RequestCountMetric
{
    public static readonly string Name = "StatusNamma.ApiService";

    private readonly Counter<int> _requestCount;

    public RequestCountMetric(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(Name);

        _requestCount = meter.CreateCounter<int>("request.count");
    }

    public void RequestReceived()
    {
        _requestCount.Add(1);
    }
}