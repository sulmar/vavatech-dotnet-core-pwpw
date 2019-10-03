using System;

namespace Vavatech.DotnetCore.Models
{

    public class Customer : BaseEntity<int>, ICustomer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Pesel { get; set; }
        public IAddress HomeAddress { get; set; }
        public IAddress InvoiceAddress { get; set; }
        public bool IsRemoved { get; set; }
    }


    public class Address : IAddress
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
    }
}
