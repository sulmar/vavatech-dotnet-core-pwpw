using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vavatech.DotnetCore.Models;

namespace Vavatech.DotnetCore.IRepositories
{
    public interface ISenderService
    {
        Task Send(ICustomer customer);
    }
}
