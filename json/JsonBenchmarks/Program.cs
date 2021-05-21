using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Dapper;

namespace JsonBenchmarks {
	class Program {
		static void Main(string[] args) {
			using var sql = new SqlConnection("Server=localhost;Database=Autobarn;User=autobarn;Password=autobarn;MultipleActiveResultSets=true;");
			sql.Open();
			var vehicles = sql.Query<Vehicle>("SELECT * FROM Vehicles").ToList();

			foreach (var v in vehicles) {
				var json = Newtonsoft.Json.JsonConvert.SerializeObject(v);
				var output = Newtonsoft.Json.JsonConvert.DeserializeObject<Vehicle>(json);
			}
			foreach (var v in vehicles) {
				var json = System.Text.Json.JsonSerializer.Serialize(v);
				var output = System.Text.Json.JsonSerializer.Deserialize<Vehicle>(json);
			}

			for (var run = 0; run < 10; run++) {

				var sw = new Stopwatch();
				sw.Start();
				for (var i = 0; i < 100; i++) {
					foreach (var v in vehicles) {
						var json = Newtonsoft.Json.JsonConvert.SerializeObject(v);
					}
				}
				sw.Stop();
				Console.WriteLine($"Serialize Newtonsoft.Json: {sw.ElapsedMilliseconds}");
				sw.Reset();
				sw.Start();
				for (var i = 0; i < 100; i++) {
					foreach (var v in vehicles) {
						var json = System.Text.Json.JsonSerializer.Serialize(v);
					}
				}
				sw.Stop();
				Console.WriteLine($"Serialize System.Text.Json: {sw.ElapsedMilliseconds}");
				sw.Reset();

				var nsInput = vehicles.Select(v => Newtonsoft.Json.JsonConvert.SerializeObject(v));
				var stInput = vehicles.Select(v => System.Text.Json.JsonSerializer.Serialize(v));

				sw.Start();
				for (var i = 0; i < 100; i++) {
					foreach (var json in nsInput) {
						var output = Newtonsoft.Json.JsonConvert.DeserializeObject<Vehicle>(json);
					}
				}
				sw.Stop();
				Console.WriteLine($"Deserialize Newtonsoft.Json: {sw.ElapsedMilliseconds}");
				sw.Reset();
				sw.Start();
				for (var i = 0; i < 100; i++) {
					foreach (var json in stInput) {
						var output = System.Text.Json.JsonSerializer.Deserialize<Vehicle>(json);
					}
				}
				sw.Stop();
				Console.WriteLine($"Deserialize System.Text.Json: {sw.ElapsedMilliseconds}");
				sw.Reset();
			}

		}
	}




	public class Vehicle {
		public string Registration { get; set; }
		public string ModelCode { get; set; }
		public string Color { get; set; }
		public int? Year { get; set; }
	}
}
