using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace Autobarn.Data.Entities {
	public partial class Vehicle {
		public string Registration { get; set; }
		public string ModelCode { get; set; }
		public string Color { get; set; }
		public int? Year { get; set; }
		[JsonIgnore]
		[Newtonsoft.Json.JsonIgnore]
		public virtual Model VehicleModel { get; set; }
	}
}
