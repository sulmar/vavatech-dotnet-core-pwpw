using Bogus;
using Vavatech.DotnetCore.Models;

namespace Vavatech.DotnetCore.Fakers
{
    public class AddressFaker : Faker<Address>
    {
        public AddressFaker()
        {
            StrictMode(true);
            RuleFor(p => p.Street, f => f.Address.StreetName());
            RuleFor(p => p.City, f => f.Address.City());
            RuleFor(p => p.Country, f => f.Address.Country());
        }
    }
}
