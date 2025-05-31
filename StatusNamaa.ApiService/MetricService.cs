namespace StatusNamaa.ApiService;

internal sealed class MetricService
{
    public async Task<double> GetCpuUsageAsync()
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

    public long GetRamUsage()
    {
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Metrics/RuntimeMetrics.cs#L40
        var memoryUsage = Environment.WorkingSet / GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

        return memoryUsage;
    }

    public long GetThreadPoolQueueLength()
    {
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Metrics/RuntimeMetrics.cs#L112
        return ThreadPool.PendingWorkItemCount;
    }

    public long GetLockContentions()
    {
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Metrics/RuntimeMetrics.cs#L94
        return Monitor.LockContentionCount;
    }

    public long GetExceptionCount()
    {
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Metrics/RuntimeMetrics.cs#L129
        // TODO: Read the metric dotnet.exceptions
        return 0;
    }
}