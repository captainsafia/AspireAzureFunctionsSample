using Aspire.Hosting.Azure;
using Azure.Provisioning.Storage;

var builder = DistributedApplication.CreateBuilder(args);

#pragma warning disable AZPROVISION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var storage = builder.AddAzureStorage("storage", (builder, construct, storageAccount) =>
{
    construct.Add(storageAccount.AssignRole(StorageBuiltInRole.StorageAccountContributor, construct.PrincipalTypeParameter, construct.PrincipalIdParameter));
}).RunAsEmulator();
#pragma warning restore AZPROVISION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
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
    .WithHostStorage(storage)
    .WithEnvironment(context =>
    {
        // Function's Docker images are configured to listen on privileged port 
        // :80 by default. We override that here using the environment variable
        // that is respected by the classic WebHostBuilder.
        context.EnvironmentVariables["ASPNETCORE_URLS"] = "http://+:8080";
    });

var apiService = builder.AddProject<Projects.AzureFunctionsTest_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithReference(funcApp)
    .WithReference(eventHubs)
    .WithReference(serviceBus)
    .WithReference(blob)
    .WithReference(queue);

builder.Build().Run();