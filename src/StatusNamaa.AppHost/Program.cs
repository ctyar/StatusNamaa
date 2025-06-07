var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.StatusNamaa_ApiService>("apiservice");

builder.Build().Run();