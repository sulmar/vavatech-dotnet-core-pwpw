using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vavatech.DotnetCore.Models;

namespace Vavatech.DotnetCore.SignalR.Hubs
{
    public class CustomersHub : Hub
    {

        public override Task OnConnectedAsync()
        {
            this.Groups.AddToGroupAsync(this.Context.ConnectionId, "Vavatech");

            return base.OnConnectedAsync();
        }

        public async Task CustomerAdded(Customer customer)
        {
            //  await this.Clients.All.SendAsync("Added", customer);

            await this.Clients.Groups("Vavatech").SendAsync("Added", customer);

            await this.Clients.Others.SendAsync("Added", customer);

            // this.Clients.AllExcept(this.Clients.Caller).
        }

        public async Task Ping(string message = "pong")
        {
            await this.Clients.Caller.SendAsync("ping", message);
        }

    }



}
