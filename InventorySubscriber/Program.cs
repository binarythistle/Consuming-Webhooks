using Azure.Messaging.ServiceBus;

ServiceBusClient client;
ServiceBusProcessor processor;

string connectionString = "YOUR AZURE SERVICE BUS CONECTION STRING HERE";
string topicName = "inventory";
string subscriptionName = "S1";
int count = 0;

// handle received messages
async Task MessageHandler(ProcessMessageEventArgs args)
{
    count++;
    string body = args.Message.Body.ToString();

    Console.WriteLine($"Received {count}: Message {body}");

    // complete the message. messages is deleted from the subscription. 
    await args.CompleteMessageAsync(args.Message);
}

// handle any errors when receiving messages
Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}

client = new ServiceBusClient(connectionString);

processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

Console.WriteLine("----> InventorySubscriber...");
Console.WriteLine("----> Setting up Azure Service Bus Connection.");


try
{
    // add handler to process messages
    processor.ProcessMessageAsync += MessageHandler;

    // add handler to process any errors
    processor.ProcessErrorAsync += ErrorHandler;

    // start processing 
    await processor.StartProcessingAsync();

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("----> Connected!");
    Console.ResetColor();    

    Console.WriteLine("\nPress any key to end the processing");
    Console.ReadKey();

    // stop processing 
    Console.WriteLine("\nStopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}
finally
{
    await processor.DisposeAsync();
    await client.DisposeAsync();
}