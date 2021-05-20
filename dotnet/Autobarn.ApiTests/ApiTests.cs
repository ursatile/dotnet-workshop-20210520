using System.Dynamic;
using Autobarn.Website;
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
	}
}