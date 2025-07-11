﻿using System.Reflection;

namespace StatusNamaa;

internal interface ICustomMetricService
{
    Task<double?> GetCpuUsageAsync();
    double? GetMemoryUsage();
    string? GetVersion();
}

internal sealed class CustomMetricService : ICustomMetricService
{
    public async Task<double?> GetCpuUsageAsync()
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

    public double? GetMemoryUsage()
    {
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Metrics/RuntimeMetrics.cs#L40
        var memoryUsage = (double)Environment.WorkingSet * 100 / GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

        return memoryUsage;
    }

    public string? GetVersion()
    {
        return Assembly.GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
    }
}