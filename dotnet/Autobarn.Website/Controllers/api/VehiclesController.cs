using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Models;
using System.Dynamic;
using Autobarn.Website.Controllers.Api;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Autobarn.Website.Messaging;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Autobarn.Website.Controllers.api {

	[Route("api/[controller]")]
	[ApiController]
	public class VehiclesController : ControllerBase {
		private IAutobarnServiceBus bus;
		private readonly IAutobarnDatabase db;

		// GET: api/vehicles
		public VehiclesController(IAutobarnDatabase db, IAutobarnServiceBus bus) {
			this.bus = bus;
			this.db = db;
		}

		private object Paginate(string href, int index, int count, int total) {
			dynamic links = new ExpandoObject();
			links.self = new {
				href
			};
			links.first = new {
				href = $"{href}?count={count}"
			};
			links.final = new {
				href = $"{href}?index={total - count}"
			};
			if (index + count < total) {
				links.next = new {
					href = $"{href}?index={index + count}&count={count}"
				};
			}
			if (index > 0) {
				links.previous = new {
					href = $"{href}?index={index - count}&count={count}"
				};
			}
			return links;
		}

		[HttpGet]
		public IActionResult Get(int index, int count = 10) {
			var items = db.ListVehicles().Skip(index).Take(count);
			var total = db.CountVehicles();
			var result = new {
				_links = Paginate("/api/vehicles", index, count, total),
				total,
				index,
				count,
				items = items.Select(v => v.ToResource())
			};
			return Ok(result);
		}

		// GET api/vehicles/5
		[HttpGet("{id}")]
		public IActionResult Get(string id) {
			var vehicle = db.FindVehicle(id);
			if (vehicle == default) return NotFound($"Sorry, there's no car with registration {id} in our system.");
			var result = vehicle.ToResource();
			return Ok(result);
		}


		// POST api/vehicles
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] VehicleDto dto) {
			// If the vehicle already exists, return a 409 Conflict
			var existing = db.FindVehicle(dto.Registration);
			if (existing != default) return Conflict($"Sorry - we already have a car with registration {dto.Registration} in our database!");
			var vehicleModel = db.FindModel(dto.ModelCode);
			var vehicle = new Vehicle {
				Registration = dto.Registration,
				Color = dto.Color,
				Year = dto.Year,
				VehicleModel = vehicleModel
			};
			db.CreateVehicle(vehicle);
			bus.PublishNewVehicleMessage(vehicle);
			return Created($"/api/vehicles/{vehicle.Registration}", dto);
		}

		// PUT api/vehicles/5
		[HttpPut("{id}")]
		public IActionResult Put(string id, [FromBody] VehicleDto dto) {
			var vehicleModel = db.FindModel(dto.ModelCode);
			var vehicle = new Vehicle {
				Registration = dto.Registration,
				Color = dto.Color,
				Year = dto.Year,
				ModelCode = vehicleModel.Code
			};
			db.UpdateVehicle(vehicle);
			return Ok(dto);
		}

		// DELETE api/vehicles/5
		[HttpDelete("{id}")]
		public IActionResult Delete(string id) {
			db.DeleteVehicle(id);
			return NoContent();
		}
	}
}
