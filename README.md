# Aspire + Azure Functions Integration Sample

[![Azure Functions Sample CI](https://github.com/captainsafia/AspireAzureFunctionsSample/actions/workflows/ci.yml/badge.svg)](https://github.com/captainsafia/AspireAzureFunctionsSample/actions/workflows/ci.yml)

This project demonstrates the Aspire and Azure Functions integration.

> [!NOTE]  
> This repository requires the following dependencies:
> - A .NET 9 RC 2 SDK to support its functionality.
> - `Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore` at v2.0.0-preview1
> - `Microsoft.Azure.Functions.Worker.Sdk` at v2.0.0-preview1
> - Azure Functions Core Tools at v4.0.6280
> - The latest preview packages of Aspire 9

## Running the project

To run this project, launch the AppHost project.

```
cd AzureFunctionsTest/AzureFunctionsTest.AppHost
dotnet run
```

Currently, there's a requirement that all Azure Functions trigger bindings specify a connection name that aligns with the name of the Aspire resource.

For example, given the following resource configuration for an Azure Storage Queue resource named "queue" and an Azure Storage Blobs resource named "blob":

```csharp
using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var queue = storage.AddQueues("queue");
var blob = storage.AddBlobs("blob");

 builder.AddAzureFunctionsProject<Projects.AzureFunctionsEndToEnd_Functions>("funcapp")
    .WithExternalHttpEndpoints()
    .WithReference(queue)
    .WithReference(blob)
```

The following trigger bindings must be used for Queue and Blob triggers respectively:

```csharp
[BlobTrigger("blobs/{name}", Connection = "blob")]
// ...
[QueueTrigger("queue", Connection = "queue")] 
```
 