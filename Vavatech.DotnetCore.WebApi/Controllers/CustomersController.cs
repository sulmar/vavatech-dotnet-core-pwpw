using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vavatech.DotnetCore.IRepositories;
using Vavatech.DotnetCore.Models;
using Vavatech.DotnetCore.Models.SearchCriterias;

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
        [HttpGet("{id:int}", Order = 0, Name = nameof(GetById))]
        public IActionResult GetById(int id)
        {
            var customer = customerRepository.Get(id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        // api/customers?city=Warszawa&country=Poland

        //[HttpGet()]
        //public IActionResult Get([FromQuery] string city, [FromQuery] string country)
        //{
        //    throw new NotImplementedException();
        //}

        // api/customers?city=Warszawa&country=Poland
        [HttpGet()]
        public IActionResult Get([FromQuery] CustomerSearchCriteria criteria)
        {

            throw new NotImplementedException();
        }

        // api/customers/PL4324325454
        [HttpGet("{pesel}", Order = 1)]
        public IActionResult Get(string pesel)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public IActionResult Post([FromBody] Customer customer)
        {
            customerRepository.Add(customer);

            // return Created($"http://localhost:5000/api/customers/{customer.Id}", customer);

            return CreatedAtRoute(nameof(GetById), new { Id = customer.Id }, customer);
        }


        // api/customers/100
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Customer customer)
        {
            if (customer.Id != id)
                return BadRequest();

            customerRepository.Update(customer);

            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            if (!customerRepository.Exists(id))
                return NotFound();

            customerRepository.Remove(id);

            return Ok();
        }
        
        [AcceptVerbs("COPY", Route ="{id}")]
        public IActionResult Copy(int id)
        {
            
            return Ok();
        }

        [HttpHead]
        public IActionResult Head(int id)
        {
            if (!customerRepository.Exists(id))
                return NotFound();

            return Ok();
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

        //[HttpGet]
        //public IActionResult Get()
        //{
        //    var customers = customerRepository.Get();

        //    return Ok(customers);
        //}
    }
}
