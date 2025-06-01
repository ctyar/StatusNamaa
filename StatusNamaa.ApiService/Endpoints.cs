namespace StatusNamaa.ApiService;

internal static class Endpoints
{
    public static async Task<string> GetSvgAsync(SvgService svgService, MetricService metricService)
    {
        var metrics = await metricService.GetMetrics();

        return svgService.GetSvg(metrics);
    }
}
