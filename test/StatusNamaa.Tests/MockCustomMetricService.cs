namespace StatusNamaa.Tests;

internal sealed class MockCustomMetricService : ICustomMetricService
{
    public Task<double?> GetCpuUsageAsync()
    {
        return Task.FromResult<double?>(100);
    }

    public double? GetMemoryUsage()
    {
        return 70;
    }

    public string? GetVersion()
    {
        return "1.2.3-alpha.6+f50922c5e0";
    }
}