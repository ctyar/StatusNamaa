namespace StatusNamaa;

internal sealed class MetricService
{
    public async Task<List<MetricDisplayItem>> GetMetrics()
    {
        var cpuUsage = await GetCpuUsageAsync();

        var metrics = new List<MetricDisplayItem>
        {
            new MetricDisplayItem("CPU", cpuUsage, "{0:0.##}%"),
            new MetricDisplayItem("Memory", GetMemoryUsage(), "{0:0.##}%"),
            new MetricDisplayItem("ThreadPool Queue", GetThreadPoolQueueLength()),
            new MetricDisplayItem("Lock Contentions", GetLockContentions()),
            new MetricDisplayItem("Exceptions", GetExceptionCount())
        };

        return metrics;
    }

    private static async Task<double> GetCpuUsageAsync()
    {
        const int delay = 100;
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Metrics/RuntimeMetrics.cs#L179
        var startUsage = Environment.CpuUsage.TotalTime;
        await Task.Delay(delay);
        var endUsage = Environment.CpuUsage.TotalTime;

        var pastMicroseconds = delay * 1000;
        var cpuUsage = (endUsage - startUsage).TotalMicroseconds / (pastMicroseconds * Environment.ProcessorCount);
        var percentage = cpuUsage * 100;

        return percentage > 100 ? 100 : percentage;
    }

    private static double GetMemoryUsage()
    {
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Metrics/RuntimeMetrics.cs#L40
        var memoryUsage = (double)Environment.WorkingSet * 100 / GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

        return memoryUsage;
    }

    private static long GetThreadPoolQueueLength()
    {
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Metrics/RuntimeMetrics.cs#L112
        return ThreadPool.PendingWorkItemCount;
    }

    private static long GetLockContentions()
    {
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Metrics/RuntimeMetrics.cs#L94
        return Monitor.LockContentionCount;
    }

    private static long GetExceptionCount()
    {
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Metrics/RuntimeMetrics.cs#L129
        // TODO: Read the metric dotnet.exceptions
        return 0;
    }
}