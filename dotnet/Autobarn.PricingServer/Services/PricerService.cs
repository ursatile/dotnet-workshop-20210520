using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Autobarn.PricingServer {
	public class PricerService : Pricer.PricerBase {
		
		private readonly ILogger<PricerService> _logger;
		
		public PricerService(ILogger<PricerService> logger) {
			_logger = logger;
		}

		public override Task<PriceReply> GetPrice(PriceRequest request, ServerCallContext context) {
			if (request.Manufacturer == "ASTON MARTIN") {

			}

			var reply = GetPriceForVehicle(request);
			return Task.FromResult(reply);
		}

		private PriceReply GetPriceForVehicle(PriceRequest request) {
            //TODO: calculate prices properly
			return new PriceReply {
				StatusCode = "OK",
				Price = 50,
				CurrencyCode = "GBP"
			};
		}
	}
}
