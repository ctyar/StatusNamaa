using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace StatusNamaa.Tests;

public class OpenTelemetryTests
{
    //[Fact]
    public async Task OpenTelemetryTest()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="200px" viewBox="0 0 470 200">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="180px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd">CPU</text></g>
            <g>
            <rect x="200px" y="68px" width="16px" height="16px" rx="2px" fill="#b0fd6a"/>
            <rect x="220px" y="68px" width="16px" height="16px" rx="2px" fill="#b9fa63"/>
            <rect x="240px" y="68px" width="16px" height="16px" rx="2px" fill="#caf85e"/>
            <rect x="260px" y="68px" width="16px" height="16px" rx="2px" fill="#e0f75f"/>
            <rect x="280px" y="68px" width="16px" height="16px" rx="2px" fill="#f2e95b"/>
            <rect x="300px" y="68px" width="16px" height="16px" rx="2px" fill="#f9d94b"/>
            <rect x="320px" y="68px" width="16px" height="16px" rx="2px" fill="#f9c03c"/>
            <rect x="340px" y="68px" width="16px" height="16px" rx="2px" fill="#fc9832"/>
            <rect x="360px" y="68px" width="16px" height="16px" rx="2px" fill="#fc3e21"/>
            <rect x="380px" y="68px" width="16px" height="16px" rx="2px" fill="#f30b0b"/>
            </g>
            <text x="460px" y="84px" fill="#f30b0b" text-anchor="end">100%</text>
            </g>
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="108px" fill="#53b1fd">Memory</text></g>
            <g>
            <rect x="200px" y="92px" width="16px" height="16px" rx="2px" fill="#b0fd6a"/>
            <rect x="220px" y="92px" width="16px" height="16px" rx="2px" fill="#b9fa63"/>
            <rect x="240px" y="92px" width="16px" height="16px" rx="2px" fill="#caf85e"/>
            <rect x="260px" y="92px" width="16px" height="16px" rx="2px" fill="#e0f75f"/>
            <rect x="280px" y="92px" width="16px" height="16px" rx="2px" fill="#f2e95b"/>
            <rect x="300px" y="92px" width="16px" height="16px" rx="2px" fill="#f9d94b"/>
            <rect x="320px" y="92px" width="16px" height="16px" rx="2px" fill="#f9c03c"/>
            <rect x="340px" y="92px" width="16px" height="16px" rx="2px" fill="#fc9832"/>
            </g>
            <text x="460px" y="108px" fill="#fc9832" text-anchor="end">70%</text>
            </g>
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="132px" fill="#53b1fd">ThreadPool</text></g>
            <text x="460px" y="132px" fill="#b0fd6a" text-anchor="end">5</text>
            </g>
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="156px" fill="#53b1fd">Lock Contention</text></g>
            <text x="460px" y="156px" fill="#b0fd6a" text-anchor="end">5</text>
            </g>
            </g>
            <text x="10px" y="180px" fill="#53b1fd" font-size="10px">Environment: <tspan fill="#b0fd6a">Production</tspan>  Version: <tspan fill="#b0fd6a">1.2.3-alpha.6+f50922c5e0</tspan></text>
            </svg>
            """;
        expected = expected.Replace("\r\n", Environment.NewLine);

        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                metrics.AddStatusNamaa();
            });

        builder.Services.AddStatusNamaa(o =>
        {
            o.AddMetric("kestrel.queued_requests");
        });
        //builder.Services.AddSingleton<ICustomMetricService, MockCustomMetricService>();
        //builder.Services.AddSingleton<IListenerService, MockListenerService>();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }
}