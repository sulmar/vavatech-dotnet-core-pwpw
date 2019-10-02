using System;

namespace Vavatech.DotnetCore.Models
{

    public class Customer : BaseEntity<int>, ICustomer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsRemoved { get; set; }
      
    }
}
