using OpenTelemetry.Metrics;

namespace StatusNamaa.ApiService;

public static class MeterProviderBuilderExtensions
{
    public static MeterProviderBuilder AddStatusNamaa(this MeterProviderBuilder builder,
        IServiceCollection services, string[] instrumentNames)
    {
        var exportedMetrics = new List<Metric>();
        builder.AddInMemoryExporter(exportedMetrics);

        var metricService = new MetricsService(exportedMetrics, instrumentNames);

        services.AddSingleton(metricService);
        services.AddSingleton<SvgService>();

        return builder;
    }
}
