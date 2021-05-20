﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autobarn.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Autobarn.Data {
	public interface IAutobarnDatabase {
		public IEnumerable<Vehicle> ListVehicles();
		public int CountVehicles();
		public IEnumerable<Manufacturer> ListManufacturers();
		public IEnumerable<Model> ListModels();

		public Vehicle FindVehicle(string registration);
		public Model FindModel(string code);
		public Manufacturer FindManufacturer(string code);

		public void CreateVehicle(Vehicle vehicle);
		public void UpdateVehicle(Vehicle vehicle);
		public void DeleteVehicle(string registration);
	}
}
