using Microsoft.AspNetCore.Mvc;

namespace StatusNamaa.ApiService;

public class Program
{
    private static readonly Lock _lock = new();

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults(RequestCountMetric.Name);

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
            var stream = SvgService.GetSvg();

            return Results.File(stream, "image/svg+xml");
        });

        app.MapGet("/lock", () =>
        {
            lock (_lock)
            {
                Thread.Sleep(10000);
            }

            return "ok";
        });

        app.MapDefaultEndpoints();

        SvgService.RegisterListener([RequestCountMetric.Name, "process.runtime.dotnet.monitor.lock_contention.count"]);

        app.Run();
    }
}
