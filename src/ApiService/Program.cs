using Microsoft.AspNetCore.Mvc;
using StatusNamaa;

namespace ApiService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddProblemDetails();

        builder.Services.AddOpenApi(o =>
        {
            o.AddDocumentTransformer((doc, context, cancellationToken) =>
            {
                doc.Info.Description = "<img src=\"statusnamaa.svg\" >";

                return Task.CompletedTask;
            });
        });
        builder.Services.AddSwaggerUI();

        builder.Services.AddSingleton<QueueLengthMetric>();
        builder.Services.AddSingleton<ListenerService>();
        builder.Services.AddStatusNamaa(o =>
        {
            o.Metrics.Add(new StatusNamaaMetric
            {
                Name = "dotnet.process.memory.working_set",
                DisplayName = "Working Set",
                Type = StatusNamaaValueType.Bytes,
            });

            o.AddMetric("My Custom Value", async services =>
            {
                var listenerService = services.GetRequiredService<ListenerService>();

                return await listenerService.GetValue();
            });

            o.AddMetric("queue.length");

            o.AddMetric("dotnet.exceptions");
        });

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

        app.MapDefaultEndpoints();

        app.MapStatusNamaa();

        app.Run();
    }
}