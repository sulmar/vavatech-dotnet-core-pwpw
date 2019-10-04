using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Vavatech.DotnetCore.Models;
using Vavatech.DotnetCore.Models.SearchCriterias;

namespace Vavatech.DotnetCore.IRepositories
{
    public interface ICustomerRepository : IEntityRepository<ICustomer, int>
    {
        IEnumerable<ICustomer> Get(string city, string street, string country, DateTime from, DateTime to);
        IEnumerable<ICustomer> Get(CustomerSearchCriteria criteria);
        bool TryAthorize(string username, string password, out ICustomer customer);
    }


    public interface ICustomerRepositoryAsync : IEntityRepositoryAsync<ICustomer, int>
    {

    }

}
