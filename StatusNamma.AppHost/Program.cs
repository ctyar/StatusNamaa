var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.StatusNamma_ApiService>("apiservice");

builder.Build().Run();
