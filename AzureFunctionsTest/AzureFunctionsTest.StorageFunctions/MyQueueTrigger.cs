using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsTest.Functions;

public class MyQueueTrigger(ILogger<MyQueueTrigger> logger)
{
    [Function(nameof(MyQueueTrigger))]
    public void Run([QueueTrigger("queue", Connection = "queue")] QueueMessage message)
    {
        logger.LogInformation("C# Queue trigger function processed: {Text}", message.MessageText);
    }
}
