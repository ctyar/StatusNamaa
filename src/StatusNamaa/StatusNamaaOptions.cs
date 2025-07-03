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
            Format = "{0:F0}%",
            Selector = services => MetricService.GetCpuUsageAsync(),
        },
        new StatusNamaaMetric
        {
            Name = "Memory",
            Format = "{0:F0}%",
            Selector = services => Task.FromResult(MetricService.GetMemoryUsage()),
        },
        new StatusNamaaMetric
        {
            Name = "ThreadPool Queue",
            Format = "{0}",
            Selector = services => Task.FromResult((double)MetricService.GetThreadPoolQueueLength()),
        },
        new StatusNamaaMetric
        {
            Name = "Lock Contentions",
            Format = "{0}",
            Selector = services => Task.FromResult((double)MetricService.GetLockContentions()),
        },
    ];


    /// <summary>
    /// Adds a custom metric to the <see cref="Metrics"/> collection with the specified name, display format, and selector function.
    /// </summary>
    /// <param name="name">The name of the metric to be displayed.</param>
    /// <param name="format">The display format string for the metric value (e.g., "{0:F0}%").</param>
    /// <param name="selector">A function that retrieves the metric value asynchronously, given an <see cref="IServiceProvider"/>.</param>
    public void AddMetric(string name, string format, Func<IServiceProvider, Task<double>> selector)
    {
        Metrics.Add(new StatusNamaaMetric
        {
            Name = name,
            Format = format,
            Selector = selector
        });
    }

    /// <summary>
    /// Adds a custom metric to the <see cref="Metrics"/> collection with the specified name and selector function.
    /// The default display format "{0}" will be used.
    /// </summary>
    /// <param name="name">The name of the metric to be displayed.</param>
    /// <param name="selector">A function that retrieves the metric value asynchronously, given an <see cref="IServiceProvider"/>.</param>
    public void AddMetric(string name, Func<IServiceProvider, Task<double>> selector)
    {
        Metrics.Add(new StatusNamaaMetric
        {
            Name = name,
            Selector = selector
        });
    }
}

/// <summary>
/// Represents a single metric definition for StatusNamaa, including its name, display format, and a selector function
/// that retrieves the metric value asynchronously. The <see cref="Name"/> property specifies the display name of the metric,
/// <see cref="Format"/> defines how the metric value will be formatted for display, and <see cref="Selector"/> is a function
/// that, given an <see cref="IServiceProvider"/>, returns the metric value as a <see cref="Task{Double}"/>.
/// </summary>
public class StatusNamaaMetric
{
    public string Name { get; set; } = string.Empty;
    public string Format { get; set; } = "{0}";
    public Func<IServiceProvider, Task<double>> Selector { get; set; } = null!;
}