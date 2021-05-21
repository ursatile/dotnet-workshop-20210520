using Autobarn.Data.Entities;
using Autobarn.Messages;
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
			var vehicleMessage = new NewVehicleAddedMessage {
				Registration = vehicle.Registration,
				Color = vehicle.Color,
				Year = vehicle.Year.Value,
				Manufacturer = vehicle.VehicleModel.Manufacturer.Name,
				Model = vehicle.VehicleModel.Name
			};
			var message = new ServiceBusMessage(JsonConvert.SerializeObject(vehicleMessage));
			await sender.SendMessageAsync(message);
		}
	}
}