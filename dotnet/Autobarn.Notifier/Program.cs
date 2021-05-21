using System;
using System.Net.Mail;
using System.Net;
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
		static ICustomerDatabase customerDatabase;

		static async Task Main(string[] args) {
			customerDatabase = new FakeCustomerDatabase();
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
            var message = JsonConvert.DeserializeObject<NewVehicleAddedMessage>(json);

            var request = new PriceRequest {
                Registration = message.Registration,
                Color = message.Color,
                Year = message.Year,
                Manufacturer = message.Manufacturer,
                Model = message.Model
            };

            var reply = grpc.GetPrice(request);
            Console.WriteLine(reply);
			await args.CompleteMessageAsync(args.Message);		
			SendCustomerEmails(message, reply);
		}

		private static void SendCustomerEmails(NewVehicleAddedMessage message, PriceReply reply) {
			var customers =  customerDatabase.GetCustomersWhoWantToGetAnEmailAboutVehicle(message, reply.Price, reply.CurrencyCode);
			var subject = $"NEW CAR! {message.Manufacturer} {message.Model} ({message.Color}, {message.Year})";
			foreach(var customer in customers) {
				var body = $@"
Dear {customer.Name},

There's a new car!

It's a {message.Manufacturer} {message.Model} ({message.Color}, {message.Year})

It costs {reply.CurrencyCode} {reply.Price}

Thanks,

Autobarn";
				SendEmail(customer.Email, subject, body);
			}
		}

		private static void SendEmail(string email, string subject, string body) {

            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("36192fbee550e6a2f", "ea20ee3fe3a9e5"),
                EnableSsl = true
            };
            client.Send("cars@autobarn.com", email, subject, body);
            Console.WriteLine($"Sent an email to {email}");
		}
	}
}
        