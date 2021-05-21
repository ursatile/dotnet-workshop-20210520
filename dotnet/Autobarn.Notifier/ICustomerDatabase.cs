using System.Collections.Generic;
using Autobarn.Messages;

namespace Autobarn.Notifier {
	interface ICustomerDatabase {
		IEnumerable<Customer> GetCustomersWhoWantToGetAnEmailAboutVehicle(NewVehicleAddedMessage message, int price, string currencyCode);
	}
}
