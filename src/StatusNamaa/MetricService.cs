using System.Reflection;

namespace StatusNamaa;

internal sealed class MetricService
{
    public static async Task<double> GetCpuUsageAsync()
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

    public static double GetMemoryUsage()
    {
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Metrics/RuntimeMetrics.cs#L40
        var memoryUsage = (double)Environment.WorkingSet * 100 / GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

        return memoryUsage;
    }

    public static string? GetVersion()
    {
        return Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
    }
}