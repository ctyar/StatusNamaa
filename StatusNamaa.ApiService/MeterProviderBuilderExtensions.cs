using OpenTelemetry.Metrics;

namespace StatusNamaa.ApiService;

public static class MeterProviderBuilderExtensions
{
    public static MeterProviderBuilder AddStatusNamaa(this MeterProviderBuilder builder, string[] instrumentNames)
    {
        builder.AddInMemoryExporter(MetricsService.ExportedMetrics);

        MetricsService.RegisterInstruments(instrumentNames);

        return builder;
    }
}
