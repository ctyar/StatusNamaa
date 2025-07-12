using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StatusNamaa.Tests.Mocks;

namespace StatusNamaa.Tests;

public class MetricDisplayServiceTests
{
    [Fact]
    public async Task CustomFormatterTest()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd"></text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end">-100-</text>
            </g>
            </g>
            <text x="10px" y="108px" fill="#53b1fd" font-size="10px">Environment: <tspan fill="#b0fd6a">Production</tspan>  Version: <tspan fill="#b0fd6a">1.2.3-alpha.6+f50922c5e0</tspan></text>
            </svg>
            """;
        expected = expected.Replace("\r\n", Environment.NewLine);

        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();

        builder.Services.AddStatusNamaa(o =>
        {
            o.Metrics.Clear();

            o.Metrics.Add(new StatusNamaaMetric
            {
                Formatter = value => $"-{value}-",
                Selector = _ => Task.FromResult<double?>(100),
            });
        });
        builder.Services.AddSingleton<ICustomMetricService, MockCustomMetricService>();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(150, "150 B")]
    [InlineData(1500, "1 KB")]
    [InlineData(1500_000, "1 MB")]
    [InlineData(1500_000_000, "1 GB")]
    public async Task ByteFormatterTest(double value, string displayValue)
    {
        var expected = $"""
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd"></text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end">{displayValue}</text>
            </g>
            </g>
            <text x="10px" y="108px" fill="#53b1fd" font-size="10px">Environment: <tspan fill="#b0fd6a">Production</tspan>  Version: <tspan fill="#b0fd6a">1.2.3-alpha.6+f50922c5e0</tspan></text>
            </svg>
            """;
        expected = expected.Replace("\r\n", Environment.NewLine);

        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();

        builder.Services.AddStatusNamaa(o =>
        {
            o.Metrics.Clear();

            o.Metrics.Add(new StatusNamaaMetric
            {
                Type = StatusNamaaValueType.Bytes,
                Selector = _ => Task.FromResult<double?>(value),
            });
        });
        builder.Services.AddSingleton<ICustomMetricService, MockCustomMetricService>();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }
}