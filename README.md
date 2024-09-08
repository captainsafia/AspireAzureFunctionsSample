# Aspire + Azure Functions Integration Sample

This project demonstrates the Aspire and Azure Functions integration.

## Running the project

This project requires the .NET 8.0.401 SDK and the Aspire workload.

```
dotnet workload install aspire
```

To run this project, launch the AppHost project.

```
cd AzureFunctionsTest/AzureFunctionsTest.AppHost
dotnet run
```

## Setting up the project

To replicate this setup on existing Azure Functions projects, a few modifications need to be made in order to work around current limitations in the integration.

1. Remove the default `AzureWebJobsStorage` configuration generated by the Azure Functions template in `local.settings.json`. The Aspire Azure Functions integration will configure the default host storage for Azure Functions using Aspire's Azure Storage integrations. These integrations handle launching the Azure Storage emulator and wiring up the endpoint references to the Azure Functions project, so no other explicit configuration is needed.

```diff
{
    "IsEncrypted": false,
    "Values": {
-        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
    }
}
```

2. Remove the default `--port` configured in the launch profile of the Azure Functions project. The Azure Functions Aspire integrations allocates a port that the Functions host will listen on. Currently, configuring two ports for the Functions host to listen on (one via the Aspire integration and one via the launch profile) is not feasible. To resolve this, remove the default port in the Functions project template and rely on Aspire to allocate it.

```diff
{
  "profiles": {
    "AzureFunctionsTest.Functions": {
      "commandName": "Project",
-      "commandLineArgs": "--port 7071",
      "launchBrowser": false
    }
  }
}
```

3. The Azure Functions .NET APIs do not currently support the `IHostApplicationBuilder` interface that Aspire's APIs depend on. For the case of ServiceDefaults APIs, it's necessary to define `IHostBuilder`-based implementations of these APIs as seen in [this file](./AzureFunctionsTest//AzureFunctionsTest.ServiceDefaults/HostBuilderExtensions.cs).

Note: OpenTelemetry from the local Functions host is not currently supported.

3. Configure the `RunCommand` properties in the project file of the Functions project to support launching the local Azure Functions host as part of the launch step for the .NET project.

```xml
<PropertyGroup>
  <RunCommand>func</RunCommand>
  <RunArguments>start --csharp</RunArguments>
</PropertyGroup>
```

4. The Azure Functions host executes an inner build when `func start` is invoked that facilitates the discovery of triggers and extensions used by the worker and wiring them up to the host. Currently, a bug in the Azure Functions Core Tools makes this inner build fail in scenarios where the Functions project has already been built (see [this issue in the Azure Functions Core Tools repo](https://github.com/Azure/azure-functions-core-tools/issues/3594)).

```
Can't determine Project to build. Expected 1 .csproj or .fsproj but found 2
```

This bug especially impacts the Azure Functions integration because the referenced Functions project is built twice:

- Once during build phase when the AppHost is launched
- Again when `func start` command is called on the target project

To workaround this issue, configure Azure Functions so that the `WorkerExtensions.csproj` that is automatically generated during the first build is emitted to a sibling directory of the Functions project. Add the following to `AzureFunctionsTest.Functions`:

```
<PropertyGroup>
    <ExtensionsCsProjDirectory>$(MSBuildProjectDirectory)/../WorkerExtensions</ExtensionsCsProjDirectory>
</PropertyGroup>
```

 