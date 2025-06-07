# StatusNamaa

[![Build Status](https://ctyar.visualstudio.com/StatusNamaa/_apis/build/status%2Fctyar.StatusNamaa?branchName=main)](https://ctyar.visualstudio.com/StatusNamaa/_build/latest?definitionId=14&branchName=main)
[![StatusNamaa](https://img.shields.io/nuget/v/StatusNamaa.svg)](https://www.nuget.org/packages/StatusNamaa/)

Easily integrate a lightweight metrics dashboard into your ASP.NET Core application. This package provides a simple yet effective way to monitor application status, accessible at `/statusnamaa.svg`.

<img src="https://raw.githubusercontent.com/ctyar/StatusNamaa/refs/heads/main/doc/images/dashboard.png" width="50%" >
<img src="https://raw.githubusercontent.com/ctyar/StatusNamaa/refs/heads/main/doc/images/fullpage.png" width="50%" >

## Usage

1. In your `Program.cs` file Add `app.AddStatusNamaa()` and `app.MapStatusNamaa()`:

    ```diff
    + builder.Services.AddStatusNamaa();

      var app = builder.Build();

    + app.MapStatusNamaa();
    ```

2. Add an image tag with `statusnamaa.svg` src in your HTML:
    ```html
    <img src="/statusnamaa.svg" alt="StatusNamaa" />
    ```

## Features
### Authentication

### Custom values

### Metrics

### Caching

## Pre-release builds

Get the package from [here](https://github.com/ctyar/StatusNamaa/pkgs/nuget/StatusNamaa).


## Build

[Install](https://get.dot.net) the [required](global.json) .NET SDK and run:
```
$ dotnet build
```