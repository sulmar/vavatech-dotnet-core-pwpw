using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using Vavatech.DotnetCore.Fakers;

namespace Vavatech.DotnetCore.SenderClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Hello Signal-R sender!");

            const string url = "http://localhost:5000/hubs/customers";

            // dotnet add package Microsoft.AspNetCore.SignalR.Client

            HubConnection connection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();

            Console.WriteLine("Connecting...");
            await connection.StartAsync();
            Console.WriteLine("Connected.");

            CustomerFaker customerFaker = new CustomerFaker(new AddressFaker());
            var customers = customerFaker.GenerateForever();

            foreach (var customer in customers)
            {
                await connection.SendAsync("CustomerAdded", customer);

                Console.WriteLine($"Sent {customer.FirstName} {customer.LastName}");

                await Task.Delay(TimeSpan.FromSeconds(1));

            }

            Console.ResetColor();
        }
    }
}
