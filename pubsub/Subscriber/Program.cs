using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Subscriber {
	class Program {
		const string BUS_CONNECTION_STRING = "Endpoint=sb://pubsub-demo-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fvr0Z5xbCwACLnRGmTWj6XZTd6y4wgjC4xEtPycXUkc=";
        const string TOPIC = "pubsub-demo-topic";
        const string SUBSCRIPTION = "pubsub-demo-subscription-1";
		static async Task Main(string[] args) {
			var client = new ServiceBusClient(BUS_CONNECTION_STRING);
            var processor = client.CreateProcessor(TOPIC, SUBSCRIPTION, new ServiceBusProcessorOptions());
            processor.ProcessMessageAsync += ProcessMessage;
            processor.ProcessErrorAsync += _ => Task.CompletedTask;
            Console.WriteLine("Waiting for messages. Press any key to exit.");
            await processor.StartProcessingAsync();
            Console.ReadKey(false);
            await processor.StopProcessingAsync();
		}

        private static async Task ProcessMessage(ProcessMessageEventArgs args) {
            Console.WriteLine(args.Message.Body);
            await args.CompleteMessageAsync(args.Message);
        }
	}
}
