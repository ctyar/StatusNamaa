using Microsoft.Extensions.DependencyInjection;

namespace StatusNamaa;

/// <summary>
/// Represents configuration options for StatusNamaa, including the metrics to be collected and reported.
/// </summary>
/// <remarks>This class allows you to define a collection of metrics that StatusNamaa will monitor and report.
/// Each metric includes a name, a display format, and a selector function that retrieves the metric value. You can add
/// custom metrics using the <see cref="AddMetric(string, string, Func{IServiceProvider, Task{double}})"/>  or <see
/// cref="AddMetric(string, Func{IServiceProvider, Task{double}})"/> methods.</remarks>
public class StatusNamaaOptions
{
    /// <summary>
    /// Gets or sets the list of metrics to be collected and reported by StatusNamaa.
    /// Each metric defines a name, display format, and a selector function that retrieves the metric value.
    /// </summary>
    public List<StatusNamaaMetric> Metrics { get; set; } =
    [
        new StatusNamaaMetric
        {
            Name = "CPU",
            Type = StatusNamaaValueType.Percentage,
            Selector = async services =>
            {
                var customMetricService = services.GetRequiredService<ICustomMetricService>();

                return await customMetricService.GetCpuUsageAsync();
            },
        },
        new StatusNamaaMetric
        {
            Name = "Memory",
            Type = StatusNamaaValueType.Percentage,
            Selector = services =>
            {
                var customMetricService = services.GetRequiredService<ICustomMetricService>();

                return Task.FromResult(customMetricService.GetMemoryUsage());
            },
        },
        new StatusNamaaMetric
        {
            Name = "dotnet.thread_pool.queue.length",
            DisplayName = "ThreadPool",
        },
        new StatusNamaaMetric
        {
            Name = "dotnet.monitor.lock_contentions",
            DisplayName = "Lock Contention",
        },
    ];

    /// <summary>
    /// Adds a custom metric to the <see cref="Metrics"/> collection with the specified name.
    /// The default display format "{0}" will be used.
    /// </summary>
    /// <param name="name">The name of the metric to be displayed.</param>
    public void AddMetric(string name)
    {
        Metrics.Add(new StatusNamaaMetric
        {
            Name = name,
        });
    }

    /// <summary>
    /// Adds a custom metric to the <see cref="Metrics"/> collection with the specified name and selector function.
    /// The default display format "{0}" will be used.
    /// </summary>
    /// <param name="name">The name of the metric to be displayed.</param>
    /// <param name="selector">A function that retrieves the metric value asynchronously, given an <see cref="IServiceProvider"/>.</param>
    public void AddMetric(string name, Func<IServiceProvider, Task<double?>> selector)
    {
        Metrics.Add(new StatusNamaaMetric
        {
            Name = name,
            Selector = selector
        });
    }

    /// <summary>
    /// Adds a custom metric to the <see cref="Metrics"/> collection with the specified name, display format, and selector function.
    /// </summary>
    /// <param name="name">The name of the metric to be displayed.</param>
    /// <param name="formatter">The display format string for the metric value (e.g., "{0:F0}%").</param>
    /// <param name="selector">A function that retrieves the metric value asynchronously, given an <see cref="IServiceProvider"/>.</param>
    public void AddMetric(string name, Func<double?, string?> formatter, Func<IServiceProvider, Task<double?>> selector)
    {
        Metrics.Add(new StatusNamaaMetric
        {
            Name = name,
            Formatter = formatter,
            Selector = selector
        });
    }

    /// <summary>
    /// Adds a custom metric to the <see cref="Metrics"/> collection with the specified name, display name, display format, and selector function.
    /// </summary>
    /// <param name="name">The internal name of the metric to be used for identification.</param>
    /// <param name="displayName">The display name of the metric to be shown in the UI or reports.</param>
    /// <param name="formatter">The display format string for the metric value (e.g., "{0:F0}%").</param>
    /// <param name="selector">A function that retrieves the metric value asynchronously, given an <see cref="IServiceProvider"/>.</param>
    public void AddMetric(string name, string displayName, Func<double?, string?> formatter, Func<IServiceProvider, Task<double?>> selector)
    {
        Metrics.Add(new StatusNamaaMetric
        {
            Name = name,
            DisplayName = displayName,
            Formatter = formatter,
            Selector = selector
        });
    }
}

/// <summary>
/// Represents a single metric definition for StatusNamaa, including its name, display format, and a selector function
/// that retrieves the metric value asynchronously. The <see cref="Name"/> property specifies the display name of the metric,
/// <see cref="Formatter"/> defines how the metric value will be formatted for display, and <see cref="Selector"/> is a function
/// that, given an <see cref="IServiceProvider"/>, returns the metric value as a <see cref="Task{Double}"/>.
/// </summary>
public class StatusNamaaMetric
{
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public StatusNamaaValueType Type { get; set; } = StatusNamaaValueType.Default;
    public Func<double?, string?>? Formatter { get; set; }
    public Func<IServiceProvider, Task<double?>>? Selector { get; set; }
}

public enum StatusNamaaValueType
{
    Default,
    Percentage,
    Bytes,
}