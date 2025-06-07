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
        builder.Services.AddSingleton<ListenerService>();
        builder.Services.AddStatusNamaa(p =>
        {
            p.Add("Queue length", "{0}", async services =>
            {
                var listenerService = services.GetRequiredService<ListenerService>();

                return await listenerService.GetValue();
            });
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

        app.MapStatusNamaa();

        app.MapDefaultEndpoints();

        app.Run();
    }
}
