namespace StatusNamaa;

internal static class Endpoints
{
    public static async Task<string> GetSvgAsync(SvgService svgService, MetricDisplayService metricDisplayService)
    {
        var metrics = await metricDisplayService.GetMetrics();

        return svgService.GetSvg(metrics);
    }
}