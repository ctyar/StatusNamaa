using OpenTelemetry.Metrics;

namespace StatusNamaa;

internal sealed class OpenTelemetryInMemoryExporter
{
    public static List<Metric> ExportedMetrics { get; set; } = [];
}