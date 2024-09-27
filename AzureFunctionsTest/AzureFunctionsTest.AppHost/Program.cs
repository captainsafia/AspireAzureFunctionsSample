using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var queue = storage.AddQueues("queue");
var blob = storage.AddBlobs("blob");

var eventHubs = builder.AddAzureEventHubs("eventhubs").RunAsEmulator().AddEventHub("myhub");
var serviceBus = builder.AddAzureServiceBus("messaging").AddQueue("myqueue");

var funcApp = builder.AddAzureFunctionsProject<Projects.AzureFunctionsTest_Functions>("funcapp")
    .WithExternalHttpEndpoints()
    .WithReference(eventHubs)
    .WithReference(serviceBus)
    .WithReference(blob)
    .WithReference(queue)
    .WithEnvironment(context =>
    {
        // Function's Docker images are configured to listen on privileged port 
        // :80 by default. We override that here using the environment variable
        // that is respected by the classic WebHostBuilder.
        context.EnvironmentVariables["ASPNETCORE_URLS"] = "http://+:8080";
    });

var apiService = builder.AddProject<Projects.AzureFunctionsTest_ApiService>("apiservice")
    .WithReference(funcApp)
    .WithReference(eventHubs)
    .WithReference(serviceBus)
    .WithReference(blob)
    .WithReference(queue);

builder.Build().Run();