using System;
using System.Net.Http;
using System.Threading.Tasks;
// using CatalogApi;
using GrpcGreeter;
using GrpcBasket;
using Grpc.Net.Client;
using GrpcCatalog;
using HelloReply = GrpcGreeter.HelloReply;


namespace GrpcGreeterClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Console.WriteLine("Press any key to exit...");

            // await Greeter();

            await Basket();
            await Catalog();

            // Console.WriteLine("Press any key to exit...");
            // var key = Console.ReadKey();
            // if (key.KeyChar.ToString() == string.Empty)
            //     Console.ReadKey();

           await Read();
        }

        private static async Task Read()
        {
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    await Catalog();
                    await Basket();
                    continue;
                }
                break;
            }
        }
        
        private static async Task Catalog()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:44323");
             var client = new Catalog.CatalogClient(channel);
            var reply = await client.SayHelloAsync(new GrpcCatalog.HelloRequest {Name = "CatalogClient"});
            Console.WriteLine("Greeting: " + reply.Message);
        }

        private static async Task Basket()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:44376");
            var client = new Basket.BasketClient(channel);
            var reply = await client.SayHelloAsync(new GrpcBasket.HelloRequest {Name = "BasketClient"});
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