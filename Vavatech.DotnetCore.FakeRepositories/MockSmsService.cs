using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vavatech.DotnetCore.IRepositories;
using Vavatech.DotnetCore.Models;

namespace Vavatech.DotnetCore.FakeRepositories
{
    public class MockSmsService : ISenderService
    {
        public Task Send(ICustomer customer)
        {   
            throw new NotImplementedException();
        }
    }

    public class MockFacebookService : ISenderService
    {
        public Task Send(ICustomer customer)
        {
            throw new NotImplementedException();
        }
    }
}
