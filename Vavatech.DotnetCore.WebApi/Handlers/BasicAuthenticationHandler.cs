using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Vavatech.DotnetCore.IRepositories;
using Vavatech.DotnetCore.Models;

namespace Vavatech.DotnetCore.WebApi.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ICustomerRepository customerRepository;
   
        public BasicAuthenticationHandler(ICustomerRepository customerRepository, 
            IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            this.customerRepository = customerRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("header Authorization missing");

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

            if (authHeader.Scheme!="Basic")
                return AuthenticateResult.Fail("Schema not supported");

            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            string[] credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');

            if (!customerRepository.TryAthorize(credentials[0], credentials[1], out ICustomer customer ))
            {
                return AuthenticateResult.Fail("Invalid username or password");
            }

            ClaimsIdentity identity = new ClaimsIdentity("Basic");
            identity.AddClaim(new Claim(ClaimTypes.Email, customer.Email));

            identity.AddClaim(new Claim(ClaimTypes.Role, "Developer"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "Trainer"));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, "Basic");

            return AuthenticateResult.Success(ticket);
        }
    }
}
