using Microsoft.AspNetCore.Mvc;

namespace Autobarn.Website.Controllers.api {
	[Route("api")]
	[ApiController]
	public class ApiController : ControllerBase {
		[HttpGet]
		public IActionResult Get() {
			var result = new {
				_links = new {
					vehicles = new {
						href = "/api/vehicles"
					}
				},
				_actions = new {
					update = new {
						name = "Create/update a vehicle",
						href = "/api/vehicles/{id}",
						method = "PUT",
						contentType = "application/json"
					}
				}
			};
			return Ok(result);
		}
	}
}