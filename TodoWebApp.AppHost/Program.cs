var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.TodoWebApp_ApiService>("apiservice");

builder.AddProject<Projects.TodoWebApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
