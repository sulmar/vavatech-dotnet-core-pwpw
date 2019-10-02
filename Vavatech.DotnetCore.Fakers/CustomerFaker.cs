using Bogus;
using System;
using Vavatech.DotnetCore.Models;

namespace Vavatech.DotnetCore.Fakers
{

    // dotnet add package Bogus
    public class CustomerFaker : Faker<Customer>
    {
        public CustomerFaker()
        {
            StrictMode(true);
            // UseSeed(1);
            RuleFor(p => p.Id, f => f.IndexFaker);
            RuleFor(p => p.FirstName, f => f.Person.FirstName);
            RuleFor(p => p.LastName, f => f.Person.LastName);
            RuleFor(p => p.IsRemoved, f => f.Random.Bool(0.3f));
            RuleFor(p => p.Email, (f, c) => $"{c.FirstName}.{c.LastName}@vavatech.pl");
        }
    }
}
