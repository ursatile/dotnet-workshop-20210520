using System;
using System.Threading.Tasks;
using Autobarn.Messages;
using Autobarn.PricingServer;
using Azure.Messaging.ServiceBus;
using Grpc.Net.Client;
using Newtonsoft.Json;

namespace Autobarn.Notifier {
	class Program {
		const string BUS_CONNECTION_STRING = "Endpoint=sb://autobarn-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=k92YyE1Syn/LuO1JYCstXEEQKoE1Q3QiRbUUlG1bfds=";
		const string TOPIC = "autobarn-new-vehicle-topic";
		const string SUBSCRIPTION = "autobarn-notifier-subscription";

        const string GRPC_URL = "https://workshop.ursatile.com:5003";

        static Pricer.PricerClient grpc;

		static async Task Main(string[] args) {
            using var channel = GrpcChannel.ForAddress(GRPC_URL);
            grpc = new Pricer.PricerClient(channel);
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
            var json = args.Message.Body.ToString();
            var m = JsonConvert.DeserializeObject<NewVehicleAddedMessage>(json);

            var request = new PriceRequest {
                Registration = m.Registration,
                Color = m.Color,
                Year = m.Year,
                Manufacturer = m.Manufacturer,
                Model = m.Model
            };

            var reply = grpc.GetPrice(request);
            Console.WriteLine(reply);
			await args.CompleteMessageAsync(args.Message);
		}
	}
}
