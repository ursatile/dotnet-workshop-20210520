using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Autobarn.Website;
using Autobarn.Website.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

namespace Autobarn.ApiTests {
	public class ApiTests : IClassFixture<WebApplicationFactory<Startup>> {

		private readonly WebApplicationFactory<Startup> factory;

		public ApiTests(WebApplicationFactory<Startup> factory) {
			this.factory = factory;
		}

		[Fact]
		public async void ApiHomepageReturnsOK() {
			var client = factory.CreateClient();
			var response = await client.GetAsync("/api");
			var status = response.StatusCode;
			((int)status).ShouldBe(200);
		}

		[Fact]
		public async void ApiDiscoveryEndpointIncludesLinks() {
			var client = factory.CreateClient();
			var response = await client.GetAsync("/api");
			var json = await response.Content.ReadAsStringAsync();
			var thing = JsonConvert.DeserializeObject<dynamic>(json);
			Assert.NotNull(thing._links);
		}

		[Fact]
		public async void ApiDiscoveryEndpointIncludesVehiclesLink() {
			var client = factory.CreateClient();
			var response = await client.GetAsync("/api");
			var json = await response.Content.ReadAsStringAsync();
			var thing = JsonConvert.DeserializeObject<dynamic>(json);
			Assert.NotNull(thing._links);
			var vehiclesLink = thing._links.vehicles;
			Assert.NotNull(vehiclesLink);
		}

		[Fact]
		public async void VehiclesLinkPointsToValidEndpoint() {
			var client = factory.CreateClient();
			var response = await client.GetAsync("/api");
			var json = await response.Content.ReadAsStringAsync();
			dynamic data = JsonConvert.DeserializeObject(json);
			var href = (string)data._links.vehicles.href;
			var vehicleJson = await client.GetStringAsync(href);
			dynamic vehicles = JsonConvert.DeserializeObject(vehicleJson);
			Assert.True(vehicles.total > 0);
		}

		private async Task<string> PutVehicle(HttpClient client, string registration, int year, string color,
			string modelCode) {
			var api = await client.GetStringAsync("/api");
			dynamic hal = JsonConvert.DeserializeObject(api);
			var href = (string) hal._actions.update.href;
			href = href.Replace("{id}", registration);
			var vehicle = new {
				registration,
				year,
				color,
				modelCode
			};
			var json = JsonConvert.SerializeObject(vehicle);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await client.PutAsync(href, content);
			response.EnsureSuccessStatusCode();
			return href;
		}

		[Fact]
		public async void PutCarViaApi() {
			var client = factory.CreateClient();
			var vehicleUri = await PutVehicle(client, "TEST1234", 1985, "Red", "volkswagen-beetle");
			var response = await client.GetStringAsync(vehicleUri);
			var vehicle = JsonConvert.DeserializeObject<dynamic>(response);

			Assert.Equal(((string)vehicle.Color), "Red");
			
			await client.DeleteAsync(vehicleUri);
		}
	}
}