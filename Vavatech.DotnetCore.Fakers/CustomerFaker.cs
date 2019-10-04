using Bogus;
using System;
using Vavatech.DotnetCore.Models;

namespace Vavatech.DotnetCore.Fakers
{

    // dotnet add package Bogus
    public class CustomerFaker : Faker<Customer>
    {
        public CustomerFaker(AddressFaker addressFaker)
        {
            StrictMode(true);
            UseSeed(1);
            RuleFor(p => p.Id, f => f.IndexFaker);
            RuleFor(p => p.FirstName, f => f.Person.FirstName);
            RuleFor(p => p.LastName, f => f.Person.LastName);
            RuleFor(p => p.Pesel, f => $"PL{f.Commerce.Ean13()}");
            RuleFor(p => p.IsRemoved, f => f.Random.Bool(0.3f));
            RuleFor(p => p.HomeAddress, f => addressFaker.Generate());
            RuleFor(p => p.InvoiceAddress, f => addressFaker.Generate());
            RuleFor(p => p.Email, (f, c) => $"{c.FirstName}.{c.LastName}@vavatech.pl");
            RuleFor(p => p.UserName, f => f.Person.UserName);
            RuleFor(p => p.HashPassword, f => "12345");
        }
    }
}
