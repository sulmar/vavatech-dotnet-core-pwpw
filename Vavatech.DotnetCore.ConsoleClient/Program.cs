using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Vavatech.DotnetCore.ApiRepositories;
using Vavatech.DotnetCore.Fakers;
using Vavatech.DotnetCore.IRepositories;
using Vavatech.DotnetCore.Models;

namespace Vavatech.DotnetCore.ConsoleClient
{
    class Program
    {

        // <= C# 7.0

        /*
        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        static async Task MainAsync(string[] args)
        {

        }

        */

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        // dotnet add package Microsoft.Extensions.DependencyInjection
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello .NET Core!");

            // OverflowTest();
            // DivideByZeroTest();

            IServiceCollection services = new ServiceCollection();

            services.AddScoped<ICustomerRepositoryAsync, ApiCustomerRepository>();

            // Microsoft.Extensions.Http
            services.AddHttpClient<ICustomerRepositoryAsync, ApiCustomerRepository>(
                client =>
                {
                    client.BaseAddress = new Uri("http://localhost:5000");
                }).AddPolicyHandler(GetRetryPolicy());

           

            using(ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                await AddCustomerTest(serviceProvider.GetService<ICustomerRepositoryAsync>());
                await GetCustomersTest(serviceProvider.GetService<ICustomerRepositoryAsync>());

            }


       
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

        }

        private static async Task AddCustomerTest(ICustomerRepositoryAsync customerRepository)
        {
          //  ICustomerRepositoryAsync customerRepository = new ApiCustomerRepository(new HttpClient());

            CustomerFaker customerFaker = new CustomerFaker(new AddressFaker());
            Customer customer = customerFaker.Generate();
            await customerRepository.AddAsync(customer);
        }

        private static async Task GetCustomersTest(ICustomerRepositoryAsync customerRepository)
        {
            //ICustomerRepositoryAsync customerRepository = new ApiCustomerRepository(new HttpClient());

            var customers = await customerRepository.GetAsync();

            foreach (var customer in customers)
            {
                Console.WriteLine(customer.FirstName);
            }
        }

        private static void OverflowTest()
        {
            byte x = 255;

            checked
            {
                x++;
                x++;
            }
        }

        private static void DivideByZeroTest()
        {
            float x = 10;
            float y = 0;

            float result = x / y;
        }
    }
}
