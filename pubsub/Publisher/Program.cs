using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Publisher {
	class Program {
		const string BUS_CONNECTION_STRING = "Endpoint=sb://pubsub-demo-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fvr0Z5xbCwACLnRGmTWj6XZTd6y4wgjC4xEtPycXUkc=";
		static async Task Main(string[] args) {
			var i = 0;
			var client = new ServiceBusClient(BUS_CONNECTION_STRING);
			var sender = client.CreateSender("pubsub-demo-topic");
			while (true) {
                Console.WriteLine("Press a key to send a message:");
                Console.ReadKey(false);
				var body = $"Message #{i++} from Dylan at {DateTime.Now}";
				var message = new ServiceBusMessage(body);                
				await sender.SendMessageAsync(message);
                Console.WriteLine($"Sent: {body}");
			}
		}
	}
}
