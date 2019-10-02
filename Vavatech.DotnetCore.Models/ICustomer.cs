namespace Vavatech.DotnetCore.Models
{
    public interface ICustomer : IEntity<int>
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        bool IsRemoved { get; set; }
    }
}
