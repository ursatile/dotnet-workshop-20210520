using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Models;


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
		[HttpGet]
		public IEnumerable<Vehicle> Get() {
			return db.ListVehicles();
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
