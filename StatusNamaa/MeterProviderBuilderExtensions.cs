using Microsoft.Extensions.DependencyInjection;

namespace StatusNamaa;

public static class MeterProviderBuilderExtensions
{
    public static IServiceCollection AddStatusNamaa(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<MetricService>();
        services.AddSingleton<SvgService>();

        return services;
    }

    public static IServiceCollection AddStatusNamaa(this IServiceCollection services, Action<StatusNamaaOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure<StatusNamaaOptions>(options =>
        {
            configureOptions(options);
        });

        services.AddSingleton<MetricService>();
        services.AddSingleton<SvgService>();

        return services;
    }
}

public class StatusNamaaOptions
{
    public List<StatusNamaaOptionsItem> Metrics { get; set; } = [];

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