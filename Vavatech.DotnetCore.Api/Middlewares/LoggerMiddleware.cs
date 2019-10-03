using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Vavatech.DotnetCore.Api.Middlewares
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate next;

        public LoggerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Trace.WriteLine($"{nameof(LoggerMiddleware)} request: {context.Request.Method} {context.Request.Path}");

            await next.Invoke(context);

            Trace.WriteLine($"{nameof(LoggerMiddleware)} response: {context.Response.StatusCode}");
        }
    }
    

    public static class LoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogger(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggerMiddleware>();
        }
    }
}
