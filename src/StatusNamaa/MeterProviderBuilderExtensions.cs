using Microsoft.Extensions.DependencyInjection;

namespace StatusNamaa;

/// <summary>
/// Provides extension methods for registering StatusNamaa services and configuration in an <see cref="IServiceCollection"/>.
/// These methods allow you to add the core StatusNamaa metric display and SVG rendering services, as well as configure
/// custom metrics to be collected and reported via <see cref="StatusNamaaOptions"/>.
/// </summary>
public static class MeterProviderBuilderExtensions
{
    /// <summary>
    /// Registers the core StatusNamaa services required for metric display and SVG rendering.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that multiple calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <c>null</c>.</exception>
    public static IServiceCollection AddStatusNamaa(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<MetricDisplayService>();
        services.AddSingleton<SvgService>();

        return services;
    }

    /// <summary>
    /// Registers the core StatusNamaa services and allows configuration of <see cref="StatusNamaaOptions"/> for custom metrics.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configureOptions">An action to configure <see cref="StatusNamaaOptions"/>, such as adding custom metrics.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that multiple calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="services"/> or <paramref name="configureOptions"/> is <c>null</c>.
    /// </exception>
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