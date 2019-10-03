namespace Vavatech.DotnetCore.Models
{
    public interface ICustomer : IEntity<int>
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        bool IsRemoved { get; set; }
        IAddress HomeAddress { get; set; }
        IAddress InvoiceAddress { get; set; }
    }

    public interface IAddress
    {
        string City { get; set; }
        string Street { get; set; }
        string Country { get; set; }
    }
}
