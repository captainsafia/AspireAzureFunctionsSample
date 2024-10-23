using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace AzureFunctionsTest.Functions;

public class MyHttpTrigger(ILogger<MyHttpTrigger> logger, CosmosClient cosmosClient)
{
    [Function("MyHttpTrigger")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");
        logger.LogInformation("CosmosClient Endpoint: {0}", cosmosClient.Endpoint);
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}

