using OpenTelemetry.Metrics;

namespace StatusNamaa.ApiService;

public static class MeterProviderBuilderExtensions
{
    public static MeterProviderBuilder AddStatusNamaa(this MeterProviderBuilder builder,
        IServiceCollection services)
    {
        //var exportedMetrics = new List<Metric>();
        //builder.AddInMemoryExporter(exportedMetrics);

        //var metricService = new ListenerService(exportedMetrics, instrumentNames);

        services.AddSingleton<MetricService>();
        services.AddSingleton<SvgService>();

        return builder;
    }
}
