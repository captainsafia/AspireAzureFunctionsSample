using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var queue = storage.AddQueues("queue");
var blob = storage.AddBlobs("blob");

var eventHubs = builder.AddAzureEventHubs("eventhubs").RunAsEmulator().AddEventHub("myhub");

var funcApp = builder.AddAzureFunctionsProject<Projects.AzureFunctionsTest_Functions>("funcapp")
    .WithExternalHttpEndpoints()
    .WithReference(eventHubs)
    .WithReference(blob)
    .WithReference(queue);

var apiService = builder.AddProject<Projects.AzureFunctionsTest_ApiService>("apiservice")
    .WithReference(funcApp)
    .WithReference(eventHubs)
    .WithReference(blob)
    .WithReference(queue);

builder.Build().Run();