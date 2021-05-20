using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Models;
using System.Dynamic;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Autobarn.Website.Controllers.api {

	[Route("api/[controller]")]
	[ApiController]
	public class VehiclesController : ControllerBase {
		private readonly IAutobarnDatabase db;

		// GET: api/vehicles
		public VehiclesController(IAutobarnDatabase db) {
			this.db = db;
		}

		private object Paginate(string href, int index, int count, int total) {
			dynamic links = new ExpandoObject();
			links.self = new {
				href
			};
			if (index + count < total) {
				links.next = new {
					href = $"{href}?index={index + count}&count={count}"
				};
				links.final = new {
					href = $"{href}?index={total - count}"
				};
			}
			if (index > 0) {
				links.previous = new {
					href = $"{href}?index={index - count}&count={count}"
				};
				links.first = new {
					href = $"{href}?count={count}"
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
				items
			};
			return Ok(result);
		}

		// GET api/vehicles/5
		[HttpGet("{id}")]
		public Vehicle Get(string id) {
			return db.FindVehicle(id);
		}

		// POST api/vehicles
		[HttpPost]
		public IActionResult Post([FromBody] VehicleDto dto) {
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
			return Ok(id);
		}
	}
}
