using System;
using System.Net.Http;
using System.Threading.Tasks;
using GrpcGreeter;
using GrpcBasket;
using Grpc.Net.Client;
using HelloReply = GrpcGreeter.HelloReply;


namespace GrpcGreeterClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Press any key to exit...");
            
            await Greeter();
            
            await Basket();
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task Basket()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:44376");
            var client =  new Basket.BasketClient(channel);
            var reply = await client.SayHelloAsync(new GrpcBasket.HelloRequest { Name = "BasketClient" });
            Console.WriteLine("Greeting: " + reply.Message);
        }

        private static async Task Greeter()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5002");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new GrpcGreeter.HelloRequest {Name = "GreeterClient"});
            Console.WriteLine("Greeting: " + reply.Message);
        }
    }
}
