using Microsoft.AspNetCore.Mvc;

namespace StatusNamaa.ApiService;

public static class StatusNamaaEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Register endpoints onto the current application for resolving the StatusNamaa associated
    /// with the current application.
    /// </summary>
    /// <param name="routeBuilder">The <see cref="IEndpointRouteBuilder"/>.</param>
    /// <returns>An <see cref="IEndpointRouteBuilder"/> that can be used to further customize the endpoints.</returns>
    public static RouteHandlerBuilder MapStatusNamaa(this IEndpointRouteBuilder routeBuilder)
    {
        var endpoint = routeBuilder.MapGet("statusnamaa.svg",
            async ([FromServices] SvgService svgService, [FromServices] MetricService metricService) =>
                Results.Content(await Endpoints.GetSvgAsync(svgService, metricService), "image/svg+xml"));

        endpoint.ExcludeFromDescription();

        return endpoint;
    }
}
