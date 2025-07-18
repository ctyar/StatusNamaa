﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace StatusNamaa;

/// <summary>
/// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to register StatusNamaa's endpoint.
/// </summary>
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
            async ([FromServices] SvgService svgService, [FromServices] MetricDisplayService metricDisplayService) =>
                Results.Content(await Endpoints.GetSvgAsync(svgService, metricDisplayService), "image/svg+xml"));

        // Just to trigger the ListenerService's ctor
        routeBuilder.ServiceProvider.GetRequiredService<IListenerService>();

        endpoint.ExcludeFromDescription();

        return endpoint;
    }
}