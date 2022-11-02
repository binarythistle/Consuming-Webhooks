using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;

namespace OnlineRetailer.Webhooks
{
    public static class InventoryTrigger
    {

        static string connectionString = "YOUR AZURE SERVICE BUS CONECTION STRING HERE";
        static string topicName = "inventory";
        static ServiceBusClient client;
        static ServiceBusSender sender;

        private const int numOfMessages = 3;

        [FunctionName("InventoryTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadLineAsync();

            client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(topicName);

            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            if (!messageBatch.TryAddMessage(new ServiceBusMessage(requestBody)))
            {
                // if it is too large for the batch
                throw new Exception("The message is too large to fit in the batch.");
            }
            //}

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus topic
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of messages has been published to the topic.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

            return new OkObjectResult("Inventory Webhook Payload Received");
        }
    }
}
