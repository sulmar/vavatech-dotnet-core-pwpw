using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Vavatech.DotnetCore.WebApi
{
    public class MinAgeRequirement : IAuthorizationRequirement
    {
        public short MinimumAge { get; private set; }

        public MinAgeRequirement(short minimumAge)
        {
            this.MinimumAge = minimumAge;
        }
    }

    public class MinimumAgeHandler : AuthorizationHandler<MinAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinAgeRequirement requirement)
        {
            var user = context.User;

            var claim = context.User.FindFirst(ClaimTypes.DateOfBirth);

            if(claim != null)
            {
                var birthday = DateTime.Parse(claim?.Value);

                var isValid = birthday.AddYears(requirement.MinimumAge) <= DateTime.Today;

                if (isValid)
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
