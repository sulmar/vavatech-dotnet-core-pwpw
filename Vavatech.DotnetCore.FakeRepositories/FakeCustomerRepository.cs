using System;
using System.Collections.Generic;
using System.Linq;
using Vavatech.DotnetCore.Fakers;
using Vavatech.DotnetCore.IRepositories;
using Vavatech.DotnetCore.Models;
using Vavatech.DotnetCore.Models.SearchCriterias;

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

        public IEnumerable<ICustomer> Get(string city, string street, string country, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICustomer> Get(CustomerSearchCriteria criteria)
        {
            var results = customers.AsQueryable();

            if (!string.IsNullOrEmpty(criteria.City))
            {
                results = results.Where(c => c.HomeAddress.City == criteria.City);
            }

            if (!string.IsNullOrEmpty(criteria.Country))
            {
                results = results.Where(c => c.HomeAddress.Country == criteria.Country);
            }

            return results.ToList();

        }

        public void Remove(int id) => customers.Remove(Get(id));

        public void Update(ICustomer entity)
        {
            throw new NotImplementedException();
        }
    }
}
