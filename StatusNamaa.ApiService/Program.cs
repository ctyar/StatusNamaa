using Microsoft.AspNetCore.Mvc;

namespace StatusNamaa.ApiService;

public class Program
{
    private static readonly Lock _lock = new();

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults(RequestCountMetric.MetricName);

        builder.Services.AddProblemDetails();

        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerUI();

        builder.Services.AddSingleton<RequestCountMetric>();

        var app = builder.Build();

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
            var stream = SvgService.GetSvg([RequestCountMetric.InstrumentName, "process.runtime.dotnet.gc.allocations.size"]);

            return Results.File(stream, "image/svg+xml");
        });

        app.MapDefaultEndpoints();

        MetricsService.Init();

        app.Run();
    }
}
