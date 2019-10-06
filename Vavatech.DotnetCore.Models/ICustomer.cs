using System;

namespace Vavatech.DotnetCore.Models
{
    public interface ICustomer : IEntity<int>
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        bool IsRemoved { get; set; }
        Address HomeAddress { get; set; }
        Address InvoiceAddress { get; set; }
        string UserName { get; set; }
        string HashPassword { get; set; }
        DateTime Birthday { get; set; }
    }

    public interface IAddress
    {
        string City { get; set; }
        string Street { get; set; }
        string Country { get; set; }
    }
}
