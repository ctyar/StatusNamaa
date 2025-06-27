using Microsoft.Extensions.DependencyInjection;

namespace StatusNamaa;

public static class MeterProviderBuilderExtensions
{
    public static IServiceCollection AddStatusNamaa(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<MetricDisplayService>();
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

        services.AddSingleton<MetricDisplayService>();
        services.AddSingleton<SvgService>();

        return services;
    }
}