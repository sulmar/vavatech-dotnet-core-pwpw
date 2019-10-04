using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vavatech.DotnetCore.IRepositories;
using Vavatech.DotnetCore.Models;
using Vavatech.DotnetCore.Models.SearchCriterias;

namespace Vavatech.DotnetCore.ApiRepositories
{
    public class ApiCustomerRepository : ICustomerRepositoryAsync
    {
        private readonly HttpClient client;

        public ApiCustomerRepository(HttpClient client)
        {
            this.client = client;
        }

        public async Task AddAsync(ICustomer entity)
        {
            string url = "api/customers";

            //string json = JsonConvert.SerializeObject(entity);
            //HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            //HttpResponseMessage response = await client.PostAsync(url, content);

            HttpResponseMessage response = await client.PostAsJsonAsync(url, entity);

            var createdEntity = await response.Content.ReadAsJsonAsync<Customer>();

            entity.Id = createdEntity.Id;

            response.EnsureSuccessStatusCode();


            Trace.WriteLine(response.Headers.Location);

          
        }

        public async Task<IEnumerable<ICustomer>> GetAsync()
        {
            client.BaseAddress = new Uri("http://localhost:5000");

            string url = "api/customers";

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // string content = await response.Content.ReadAsStringAsync();
            // var customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(content);

            var customers = await response.Content.ReadAsJsonAsync<IEnumerable<Customer>>();
            return customers;
        }

        public async Task<ICustomer> GetAsync(int id)
        {
            string url = $"api/customers/{id}";

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var customer = await response.Content.ReadAsJsonAsync<Customer>();
            return customer;
        }

        public Task RemoveAsync(int id)
        {
            string url = $"api/customers/{id}";

            throw new NotImplementedException();
        }

        public async Task UpdateAsync(ICustomer entity)
        {
            string url = $"api/customers/{entity.Id}";

            HttpResponseMessage response = await client.PutAsJsonAsync(url, entity);
            response.EnsureSuccessStatusCode();
        }
    }
}
