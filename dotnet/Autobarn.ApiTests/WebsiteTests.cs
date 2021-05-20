using Autobarn.Website;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace Autobarn.ApiTests {
	public class WebsiteTests  : IClassFixture<WebApplicationFactory<Startup>> {

		private readonly WebApplicationFactory<Autobarn.Website.Startup> factory;

		public WebsiteTests(WebApplicationFactory<Autobarn.Website.Startup> factory) {
			this.factory = factory;
		}

		[Fact]
		public async void HomepageReturns200OK() {
			var client = factory.CreateClient();
			var response = await client.GetAsync("/");
			var status = response.StatusCode;
			((int)status).ShouldBe(200);
		}

		[Fact]
		public async void PrivacyReturns200OK() {
			var client = factory.CreateClient();
			var response = await client.GetAsync("/home/privacy");
			var status = response.StatusCode;
			((int)status).ShouldBe(200);
		}
	}
}