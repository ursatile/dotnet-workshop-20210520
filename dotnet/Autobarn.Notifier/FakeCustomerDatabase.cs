using System.Collections.Generic;
using Autobarn.Messages;

namespace Autobarn.Notifier {
	public class FakeCustomerDatabase : ICustomerDatabase {        
		public IEnumerable<Customer> GetCustomersWhoWantToGetAnEmailAboutVehicle(NewVehicleAddedMessage message, int price, string currencyCode) {
            yield return new Customer { Name = "Alice Andrews", Email = "alice@example.com" };
            yield return new Customer { Name = "Bob Brisket", Email = "bob@example.com" };			
		}
	}
}
