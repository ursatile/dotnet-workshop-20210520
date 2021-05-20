using System.Collections.Generic;
using System.Linq;
using Autobarn.Data.Entities;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Autobarn.Data {
	public class AutobarnSqlDatabase : IAutobarnDatabase {
		private readonly AutobarnDbContext dbContext;

		public AutobarnSqlDatabase(AutobarnDbContext dbContext) => this.dbContext = dbContext;

		// public IEnumerable<Vehicle> ListVehicles() => dbContext.Vehicles;
		// 	// .Include(v => v.VehicleModel).ThenInclude(model => model.Manufacturer);

		public IEnumerable<Vehicle> ListVehicles() {
			var conn = dbContext.Database.GetDbConnection();
			return conn.Query<Vehicle>("SELECT * FROM Vehicles");
		}

		public IEnumerable<Vehicle> ListVehicles(int index, int count) {
			var conn = dbContext.Database.GetDbConnection();
			const string SQL = @"
			select * from vehicles 
			order by registration 
			offset @index rows 
			fetch next @count rows only";
			return conn.Query<Vehicle>(SQL, new { index, count });
		}

		public IEnumerable<Manufacturer> ListManufacturers() => dbContext.Manufacturers;

		public IEnumerable<Model> ListModels() => dbContext.Models;

		public Vehicle FindVehicle(string registration) => dbContext.Vehicles.FirstOrDefault(v => v.Registration == registration);

		public Model FindModel(string code) => dbContext.Models.Find(code);

		public Manufacturer FindManufacturer(string code) => dbContext.Manufacturers.Find(code);

		public void CreateVehicle(Vehicle vehicle) {
			dbContext.Vehicles.Add(vehicle);
			dbContext.SaveChanges();
		}

		public void UpdateVehicle(Vehicle vehicle) {
			var existing = FindVehicle(vehicle.Registration);
			if (existing == default) {
				dbContext.Vehicles.Add(vehicle);
			} else {
				dbContext.Entry(existing).CurrentValues.SetValues(vehicle);
			}
			dbContext.SaveChanges();
		}

		public void DeleteVehicle(Vehicle vehicle) {
			dbContext.Vehicles.Remove(vehicle);
			dbContext.SaveChanges();
		}

		public int CountVehicles() => dbContext.Vehicles.Count();
	}
}