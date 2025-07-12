using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StatusNamaa.Tests.Mocks;

namespace StatusNamaa.Tests;

public class ListenerServiceTests
{
    [Fact]
    public async Task ObservableInstrumentTest()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd">dotnet.thread_pool.queue.length</text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end">0</text>
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
                Name = "dotnet.thread_pool.queue.length",
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

    [Fact]
    public async Task MeasurementEventWithNoEventTest()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd">CustomInstrument</text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end"></text>
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
                Name = "CustomInstrument",
            });
        });
        builder.Services.AddSingleton<ICustomMetricService, MockCustomMetricService>();
        builder.Services.AddSingleton<CustomMetric<int>>();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Fact(Skip = "This is failing at the moment because the ListenerService's ctor doesn't get triggered until the first request")]
    public async Task IntEventBeforeFirstRequestTest()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd">CustomInstrument</text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end">1</text>
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
                Name = "CustomInstrument",
            });
        });
        builder.Services.AddSingleton<ICustomMetricService, MockCustomMetricService>();
        builder.Services.AddSingleton<CustomMetric<int>>();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();
        app.Services.GetRequiredService<CustomMetric<int>>().Add(1);

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task IntEventAfterFirstRequestTest()
    {
        var expectedFirstRequest = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd">CustomInstrument</text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end"></text>
            </g>
            </g>
            <text x="10px" y="108px" fill="#53b1fd" font-size="10px">Environment: <tspan fill="#b0fd6a">Production</tspan>  Version: <tspan fill="#b0fd6a">1.2.3-alpha.6+f50922c5e0</tspan></text>
            </svg>
            """;
        var expectedSecondRequest = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd">CustomInstrument</text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end">1</text>
            </g>
            </g>
            <text x="10px" y="108px" fill="#53b1fd" font-size="10px">Environment: <tspan fill="#b0fd6a">Production</tspan>  Version: <tspan fill="#b0fd6a">1.2.3-alpha.6+f50922c5e0</tspan></text>
            </svg>
            """;
        expectedFirstRequest = expectedFirstRequest.Replace("\r\n", Environment.NewLine);
        expectedSecondRequest = expectedSecondRequest.Replace("\r\n", Environment.NewLine);

        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddStatusNamaa(o =>
        {
            o.Metrics.Clear();
            o.Metrics.Add(new StatusNamaaMetric
            {
                Name = "CustomInstrument",
            });
        });
        builder.Services.AddSingleton<ICustomMetricService, MockCustomMetricService>();
        builder.Services.AddSingleton<CustomMetric<int>>();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);
        Assert.Equal(expectedFirstRequest, actual);

        app.Services.GetRequiredService<CustomMetric<int>>().Add(1);

        actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);
        Assert.Equal(expectedSecondRequest, actual);
    }

    [Fact]
    public async Task ByteEventAfterFirstRequestTest()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd">CustomInstrument</text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end">1</text>
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
                Name = "CustomInstrument",
            });
        });
        builder.Services.AddSingleton<CustomMetric<byte>>();
        builder.Services.AddSingleton<ICustomMetricService, MockCustomMetricService>();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        app.Services.GetRequiredService<CustomMetric<byte>>().Add(1);

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task FloatEventAfterFirstRequestTest()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd">CustomInstrument</text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end">1</text>
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
                Name = "CustomInstrument",
            });
        });
        builder.Services.AddSingleton<CustomMetric<float>>();
        builder.Services.AddSingleton<ICustomMetricService, MockCustomMetricService>();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        app.Services.GetRequiredService<CustomMetric<float>>().Add(1);

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task DoubleEventAfterFirstRequestTest()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd">CustomInstrument</text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end">1</text>
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
                Name = "CustomInstrument",
            });
        });
        builder.Services.AddSingleton<ICustomMetricService, MockCustomMetricService>();
        builder.Services.AddSingleton<CustomMetric<double>>();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        app.Services.GetRequiredService<CustomMetric<double>>().Add(1);

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task DecimalEventAfterFirstRequestTest()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd">CustomInstrument</text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end">1</text>
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
                Name = "CustomInstrument",
            });
        });
        builder.Services.AddSingleton<ICustomMetricService, MockCustomMetricService>();
        builder.Services.AddSingleton<CustomMetric<decimal>>();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        app.Services.GetRequiredService<CustomMetric<decimal>>().Add(1);

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task ShortEventAfterFirstRequestTest()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd">CustomInstrument</text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end">1</text>
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
                Name = "CustomInstrument",
            });
        });
        builder.Services.AddSingleton<ICustomMetricService, MockCustomMetricService>();
        builder.Services.AddSingleton<CustomMetric<short>>();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        app.Services.GetRequiredService<CustomMetric<short>>().Add(1);

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task LongEventAfterFirstRequestTest()
    {
        var expected = """
            <svg xmlns="http://www.w3.org/2000/svg" style="background:#20242c;font-family:'Segoe UI',sans-serif;" width="470px" height="128px" viewBox="0 0 470 128">
            <clipPath id="clip1">
            <rect x="10px" y="10px" width="180px" height="108px"/>
            </clipPath>
            <text x="10px" y="36px" fill="#bfc9d1" font-size="36px" font-weight="500">Status Namaa</text>
            <g font-size="24px" font-weight="400">
            <g>
            <g clip-path="url(#clip1)"><text x="10px" y="84px" fill="#53b1fd">CustomInstrument</text></g>
            <text x="460px" y="84px" fill="#b0fd6a" text-anchor="end">1</text>
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
                Name = "CustomInstrument",
            });
        });
        builder.Services.AddSingleton<ICustomMetricService, MockCustomMetricService>();
        builder.Services.AddSingleton<CustomMetric<long>>();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        app.Services.GetRequiredService<CustomMetric<long>>().Add(1);

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);
        Assert.Equal(expected, actual);
    }

    private class CustomMetric<T> where T : struct
    {
        public static readonly string MetricName = "CustomMetric";
        public static readonly string InstrumentName = "CustomInstrument";

        private readonly UpDownCounter<T> _count;

        public CustomMetric(IMeterFactory meterFactory)
        {
            var meter = meterFactory.Create(MetricName);

            _count = meter.CreateUpDownCounter<T>(InstrumentName);
        }

        public void Add(T count)
        {
            _count.Add(count);
        }
    }
}