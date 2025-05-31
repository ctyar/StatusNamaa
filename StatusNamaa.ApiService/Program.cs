using Microsoft.AspNetCore.Mvc;

namespace StatusNamaa.ApiService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddProblemDetails();

        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerUI();

        builder.Services.AddSingleton<QueueLengthMetric>();

        var app = builder.Build();

        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapSwaggerUI();
        }

        string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

        app.MapGet("/produce", ([FromServices] QueueLengthMetric requestCountMetric) =>
        {
            requestCountMetric.Produce();

            return "done";
        });
        app.MapGet("/consume", ([FromServices] QueueLengthMetric requestCountMetric) =>
        {
            requestCountMetric.Consume();

            return "done";
        });

        app.MapGet("/statusnamma", ([FromServices] SvgService svgService) =>
        {
            var svgDoc = svgService.GetSvg();

            File.WriteAllText("C:\\Users\\ctyar\\Desktop\\new.svg", svgDoc);

            return Results.File(System.Text.Encoding.UTF8.GetBytes(svgDoc), "image/svg+xml");
        });

        app.MapGet("/statusnamma2", async ([FromServices] SvgService svgService) =>
        {
            var metricService = new MetricService();
            var cpuUsage = await metricService.GetCpuUsageAsync();

            return $"CPU: {cpuUsage} Ram: {metricService.GetMemoryUsage()}" +
                $" ThreadPoolQueueLength: {metricService.GetThreadPoolQueueLength()}" +
                $" LockContentions: {metricService.GetLockContentions()}";
        });

        app.MapDefaultEndpoints();

        app.Run();
    }
}
