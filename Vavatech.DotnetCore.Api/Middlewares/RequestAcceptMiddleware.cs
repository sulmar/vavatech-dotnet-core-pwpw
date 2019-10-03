using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vavatech.DotnetCore.Api.Middlewares
{
    public class RequestAcceptMiddleware
    {
        private readonly RequestDelegate next;

        public RequestAcceptMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var formatQuery = context.Request.Query["format"];

            if (!string.IsNullOrEmpty(formatQuery))
            {
                context.Request.Headers.Remove("Accept");
                context.Request.Headers.Append("Accept", new string[] { formatQuery });
            }

            await next(context);
        }

    }
}
