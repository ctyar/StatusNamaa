using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace StatusNamaa.Tests;

public class UnitTest1
{
    [Fact]
    public async Task DefaultOptionTest()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddStatusNamaa();

        var app = builder.Build();
        app.MapStatusNamaa();

        app.Start();
        var client = app.GetTestClient();

        var actual = await client.GetStringAsync("/statusnamaa.svg", TestContext.Current.CancellationToken);

        Assert.Equal("", actual);
    }
}
