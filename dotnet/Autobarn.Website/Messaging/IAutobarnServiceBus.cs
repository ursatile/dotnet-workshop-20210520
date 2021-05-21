using Autobarn.Data.Entities;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Autobarn.Website.Messaging {
	public interface IAutobarnServiceBus {
		void PublishNewVehicleMessage(Vehicle vehicle);
	}

	public class AutobarnAzureServiceBus : IAutobarnServiceBus {
        private ServiceBusSender sender;
        
		public AutobarnAzureServiceBus(ServiceBusClient client, string topic) {     
            sender = client.CreateSender(topic);
        }       

        public async void PublishNewVehicleMessage(Vehicle vehicle) {
			var message = new ServiceBusMessage(JsonConvert.SerializeObject(vehicle));
			await sender.SendMessageAsync(message);
		}
	}
}