using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32.SafeHandles;
using System;
using System.Threading.Tasks;
using Vavatech.DotnetCore.Models;

namespace Vavatech.DotnetCore.ReceiverClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Hello Signal-R receiver!");

            const string url = "http://localhost:5000/hubs/customers";

            // dotnet add package Microsoft.AspNetCore.SignalR.Client

            HubConnection connection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();

            Console.WriteLine("Connecting...");
            await connection.StartAsync();
            Console.WriteLine("Connected.");

            connection.On<Customer>("Added", 
                customer => Console.WriteLine($"Received {customer.FirstName} {customer.LastName}"));

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            Console.ResetColor();

        }
    }
}
