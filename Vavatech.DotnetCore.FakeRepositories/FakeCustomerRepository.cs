using System;
using System.Collections.Generic;
using System.Linq;
using Vavatech.DotnetCore.Fakers;
using Vavatech.DotnetCore.IRepositories;
using Vavatech.DotnetCore.Models;

namespace Vavatech.DotnetCore.FakeRepositories
{

    public class FakeCustomerRepository : ICustomerRepository
    {
        private readonly ICollection<ICustomer> customers;

        public FakeCustomerRepository(CustomerFaker customerFaker) 
            => this.customers = customerFaker.Generate(100).OfType<ICustomer>().ToList();

        public void Add(ICustomer entity) => customers.Add(entity);

        public IEnumerable<ICustomer> Get() => customers;

        public ICustomer Get(int id) => customers.SingleOrDefault(e => e.Id == id);

        public void Remove(int id) => customers.Remove(Get(id));

        public void Update(ICustomer entity)
        {
            throw new NotImplementedException();
        }
    }
}
