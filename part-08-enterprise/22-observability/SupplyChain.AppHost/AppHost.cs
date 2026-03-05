var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.SupplyChain_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.Build().Run();
