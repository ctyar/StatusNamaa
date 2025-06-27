namespace StatusNamaa;

public class StatusNamaaOptions
{
    public List<StatusNamaaOptionsItem> Metrics { get; set; } =
    [
        new StatusNamaaOptionsItem
        {
            Name = "CPU",
            Format = "{0:F0}%",
            Handler = services => MetricService.GetCpuUsageAsync(),
        },
        new StatusNamaaOptionsItem
        {
            Name = "Memory",
            Format = "{0:F0}%",
            Handler = services => Task.FromResult(MetricService.GetMemoryUsage()),
        },
        new StatusNamaaOptionsItem
        {
            Name = "ThreadPool Queue",
            Format = "{0}",
            Handler = services => Task.FromResult((double)MetricService.GetThreadPoolQueueLength()),
        },
        new StatusNamaaOptionsItem
        {
            Name = "Lock Contentions",
            Format = "{0}",
            Handler = services => Task.FromResult((double)MetricService.GetLockContentions()),
        },
    ];

    public void Add(string name, string format, Func<IServiceProvider, Task<double>> handler)
    {
        Metrics.Add(new StatusNamaaOptionsItem
        {
            Name = name,
            Format = format,
            Handler = handler
        });
    }
}

public class StatusNamaaOptionsItem
{
    public string Name { get; set; } = string.Empty;
    public string Format { get; set; } = "{0}";
    public Func<IServiceProvider, Task<double>> Handler { get; set; } = null!;
}