using System;
using Grpc.Net.Client;
using GrpcServer;

namespace GrpcClient {
	class Program {
		private const string GRPC_URL = "https://localhost:5001";
		static void Main(string[] args) {
            using var channel = GrpcChannel.ForAddress(GRPC_URL);
            var greeter = new Greeter.GreeterClient(channel);
            while (true) {
	            var request = new HelloRequest() {Name = "Everybody!"};
	            var reply = greeter.SayHello(request);
	            Console.WriteLine(reply.Message);
	            Console.ReadKey(false);
            }
		}
	}
}