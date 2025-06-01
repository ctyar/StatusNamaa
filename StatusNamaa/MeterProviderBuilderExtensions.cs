using Microsoft.Extensions.DependencyInjection;

namespace StatusNamaa;

public static class MeterProviderBuilderExtensions
{
    public static IServiceCollection AddStatusNamaa(this IServiceCollection services)
    {
        services.AddSingleton<MetricService>();
        services.AddSingleton<SvgService>();

        return services;
    }
}
