using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Autobarn.Notifier {
	class Program {
		const string BUS_CONNECTION_STRING = "Endpoint=sb://autobarn-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=k92YyE1Syn/LuO1JYCstXEEQKoE1Q3QiRbUUlG1bfds=";
		const string TOPIC = "autobarn-new-vehicle-topic";
		const string SUBSCRIPTION = "autobarn-notifier-subscription";

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
