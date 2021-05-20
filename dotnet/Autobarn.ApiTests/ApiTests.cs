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
			var thing = JsonConvert.DeserializeObject(json);
			//var vehiclesLink = thing["_links"]["vehicles"];
			//var href = (string)vehiclesLink["href"];
			//href.ShouldBe("/api/vehicles");
		}

		[Fact]
		public async void VehiclesLinkReturnsVehicleHypermedia() {
			var client = factory.CreateClient();
			var rawResponseBody = await client.GetStringAsync("/api/vehicles");
			rawResponseBody.ShouldStartWith("{\"_links\"");
			//dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(json);
			//Assert.True(data.total > 0);
		}
	}
}