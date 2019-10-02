using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vavatech.DotnetCore.IRepositories;

namespace Vavatech.DotnetCore.WebApi.Controllers
{
    [Route("api/customers")]
    public class CustomersV2Controller : CustomersController
    {
        public CustomersV2Controller(ICustomerRepository customerRepository)
            : base(customerRepository)
        {
        }

        // https://docs.microsoft.com/pl-pl/aspnet/core/fundamentals/routing?view=aspnetcore-2.2#route-constraint-reference
        [HttpGet("{id:int}", Order = 0)]
        public IActionResult Get(int id)
        {
            var customer = customerRepository.Get(id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpGet("{pesel}", Order = 1)]
        public IActionResult Get(string pesel)
        {
            throw new NotImplementedException();
        }
    }

    [Route("api/v1/customers")]
    public class CustomersController : ControllerBase
    {
        protected readonly ICustomerRepository customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        // GET api/customers

        // curl -X GET http://localhost:5000/api/customers
        // curl -X GET https://localhost:5001/api/customers

        [HttpGet]
        public IActionResult Get()
        {
            var customers = customerRepository.Get();

            return Ok(customers);
        }
    }
}
