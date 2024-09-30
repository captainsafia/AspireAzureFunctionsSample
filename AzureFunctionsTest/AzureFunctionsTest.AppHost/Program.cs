using Aspire.Hosting.Azure;
using Azure.Provisioning.Storage;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var queue = storage.AddQueues("queue");
var blob = storage.AddBlobs("blob");

var eventHubs = builder.AddAzureEventHubs("eventhubs").RunAsEmulator().AddEventHub("myhub");
var serviceBus = builder.AddAzureServiceBus("messaging").AddQueue("myqueue");

builder.AddAzureFunctionsProject<Projects.AzureFunctionsTest_Functions>("funcapp")
    .WithReference(eventHubs)
    .WithReference(serviceBus);

builder.AddAzureFunctionsProject<Projects.AzureFunctionsTest_StorageFunctions>("storage-funcapp")
    .WithReference(queue)
    .WithReference(blob);

var httpFuncApp = builder.AddAzureFunctionsProject<Projects.AzureFunctionsTest_HttpFunctions>("http-funcapp")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.AzureFunctionsTest_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithReference(httpFuncApp)
    .WithReference(eventHubs)
    .WithReference(serviceBus)
    .WithReference(blob)
    .WithReference(queue);

builder.Build().Run();