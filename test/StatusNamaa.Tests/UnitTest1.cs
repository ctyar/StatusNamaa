using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace StatusNamaa.Tests;

public class UnitTest1
{
    [Fact]
    public async Task DefaultOptionTest()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="480px" height="200px" viewBox="0 0 480 200">
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <text x="10px" y="84px" fill="#53b1fd">CPU</text>
            <g>
            <rect x="214px" y="68px" width="16px" height="16px" rx="2px" fill="#b0fd6a"/>
            <rect x="234px" y="68px" width="16px" height="16px" rx="2px" fill="#b9fa63"/>
            <rect x="254px" y="68px" width="16px" height="16px" rx="2px" fill="#caf85e"/>
            <rect x="274px" y="68px" width="16px" height="16px" rx="2px" fill="#e0f75f"/>
            <rect x="294px" y="68px" width="16px" height="16px" rx="2px" fill="#f2e95b"/>
            <rect x="314px" y="68px" width="16px" height="16px" rx="2px" fill="#f9d94b"/>
            <rect x="334px" y="68px" width="16px" height="16px" rx="2px" fill="#f9c03c"/>
            <rect x="354px" y="68px" width="16px" height="16px" rx="2px" fill="#fc9832"/>
            <rect x="374px" y="68px" width="16px" height="16px" rx="2px" fill="#fc3e21"/>
            <rect x="394px" y="68px" width="16px" height="16px" rx="2px" fill="#f30b0b"/>
            </g>
            <text x="470px" y="84px" fill="#f30b0b" text-anchor="end">100%</text>
            </g>
            <g>
            <text x="10px" y="108px" fill="#53b1fd">Memory</text>
            <g>
            <rect x="214px" y="92px" width="16px" height="16px" rx="2px" fill="#b0fd6a"/>
            <rect x="234px" y="92px" width="16px" height="16px" rx="2px" fill="#b9fa63"/>
            <rect x="254px" y="92px" width="16px" height="16px" rx="2px" fill="#caf85e"/>
            <rect x="274px" y="92px" width="16px" height="16px" rx="2px" fill="#e0f75f"/>
            <rect x="294px" y="92px" width="16px" height="16px" rx="2px" fill="#f2e95b"/>
            <rect x="314px" y="92px" width="16px" height="16px" rx="2px" fill="#f9d94b"/>
            <rect x="334px" y="92px" width="16px" height="16px" rx="2px" fill="#f9c03c"/>
            <rect x="354px" y="92px" width="16px" height="16px" rx="2px" fill="#fc9832"/>
            </g>
            <text x="470px" y="108px" fill="#fc9832" text-anchor="end">70%</text>
            </g>
            <g>
            <text x="10px" y="132px" fill="#53b1fd">ThreadPool Queue</text>
            <text x="470px" y="132px" fill="#b0fd6a" text-anchor="end">25</text>
            </g>
            <g>
            <text x="10px" y="156px" fill="#53b1fd">Lock Contentions</text>
            <text x="470px" y="156px" fill="#b0fd6a" text-anchor="end"></text>
            </g>
            </g>
            <text x="10px" y="180px" fill="#53b1fd" font-size="10px">Environment: <tspan fill="#b0fd6a">Production</tspan>  Version: <tspan fill="#b0fd6a">0.5.3-alpha.5+60ab110fe4</tspan></text>
            </svg>
            """;
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddStatusNamaa(o =>
        {
            o.Metrics[0].Selector = services =>
            {
                return Task.FromResult((double?)100);
            };
            o.Metrics[1].Selector = services =>
            {
                return Task.FromResult((double?)70);
            };
            o.Metrics[2].Selector = services =>
            {
                return Task.FromResult((double?)25);
            };
            o.Metrics[3].Selector = services =>
            {
                return Task.FromResult((double?)null);
            };
        });

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task EmptyMetrics()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="480px" height="104px" viewBox="0 0 480 104">
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <text x="10px" y="84px" fill="#53b1fd" font-size="10px">Environment: <tspan fill="#b0fd6a">Production</tspan>  Version: <tspan fill="#b0fd6a">0.5.3-alpha.5+60ab110fe4</tspan></text>
            </svg>
            """;
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddStatusNamaa(o =>
        {
            o.Metrics.Clear();
        });

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task MetricWithEmptyName()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="480px" height="128px" viewBox="0 0 480 128">
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <text x="10px" y="84px" fill="#53b1fd"></text>
            <text x="470px" y="84px" fill="#b0fd6a" text-anchor="end">123</text>
            </g>
            </g>
            <text x="10px" y="108px" fill="#53b1fd" font-size="10px">Environment: <tspan fill="#b0fd6a">Production</tspan>  Version: <tspan fill="#b0fd6a">0.6.0-alpha.3+1730f5cec3</tspan></text>
            </svg>
            """;
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddStatusNamaa(o =>
        {
            o.Metrics.Clear();
            o.Metrics.Add(new StatusNamaaMetric
            {
                DisplayName = null,
                Selector = services =>
                {
                    return Task.FromResult((double?)123);
                }
            });
        });

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task MetricWithLowValue()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="480px" height="128px" viewBox="0 0 480 128">
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <text x="10px" y="84px" fill="#53b1fd"></text>
            <text x="470px" y="84px" fill="#b0fd6a" text-anchor="end">-1</text>
            </g>
            </g>
            <text x="10px" y="108px" fill="#53b1fd" font-size="10px">Environment: <tspan fill="#b0fd6a">Production</tspan>  Version: <tspan fill="#b0fd6a">0.6.0-alpha.3+1730f5cec3</tspan></text>
            </svg>
            """;
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddStatusNamaa(o =>
        {
            o.Metrics.Clear();
            o.Metrics.Add(new StatusNamaaMetric
            {
                Selector = services =>
                {
                    return Task.FromResult((double?)-1);
                }
            });
        });

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task MetricWithLongName()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="480px" height="128px" viewBox="0 0 480 128">
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <text x="10px" y="84px" fill="#53b1fd"></text>
            <text x="470px" y="84px" fill="#b0fd6a" text-anchor="end">-1</text>
            </g>
            </g>
            <text x="10px" y="108px" fill="#53b1fd" font-size="10px">Environment: <tspan fill="#b0fd6a">Production</tspan>  Version: <tspan fill="#b0fd6a">0.6.0-alpha.3+1730f5cec3</tspan></text>
            </svg>
            """;
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddStatusNamaa(o =>
        {
            o.Metrics.Clear();
            o.Metrics.Add(new StatusNamaaMetric
            {
                Selector = services =>
                {
                    return Task.FromResult((double?)-1);
                }
            });
        });

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }
}
