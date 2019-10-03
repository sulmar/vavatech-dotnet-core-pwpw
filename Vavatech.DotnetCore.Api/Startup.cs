using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Owin;
using Microsoft.Extensions.DependencyInjection;
using Vavatech.DotnetCore.Api.Middlewares;

namespace Vavatech.DotnetCore.Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

          //   app.UseOwin(pipeline => pipeline(environment => OwinHandler));

            //app.UseOwin(pipeline =>
            //{
            //    pipeline(async (next) =>
            //    {
            //        // do something before
            //         await OwinHello();
            //        // do something after

            //        await next.Invoke(null);

                    
            //    });
            //});

            app.Use(async (context, next) =>
            {
                Trace.WriteLine($"request: {context.Request.Method} {context.Request.Path}");

                await next.Invoke();

                Trace.WriteLine($"response: {context.Response.StatusCode}");
            });

          

            app.Use(async (context, next) =>
            {
                Trace.WriteLine($"request 2: {context.Request.Method} {context.Request.Path}");

                await next.Invoke();

                Trace.WriteLine($"response 2: {context.Response.StatusCode}");
            });

            // app.UseMiddleware<LoggerMiddleware>();

            app.UseLogger();

            app.UseMiddleware<RequestAcceptMiddleware>();


            // Maps

            app.Map("/dashboard", HandleDashboard);

            app.Run(async (context) =>
            {
                context.Response.StatusCode = 401;

                await context.Response.WriteAsync("zboczyłes");
            });
        }

        private void HandleDashboard(IApplicationBuilder app)
        {
            app.Run(async context => await context.Response.WriteAsync("dashboard"));
        }

        private Task OwinHello()
        {
            return Task.CompletedTask;
        }

        //private Task OwinHello(OwinEnvironment owinEnvironment)
        //{
        //    var requestMethod = owinEnvironment.FeatureMaps["owin.RequestMethod"];

        //    return Task.CompletedTask;
        //}

        private Task OwinHandler(IDictionary<string, object> environment)
        {
            // http://owin.org/spec/spec/owin-1.0.0.html

            var requestMethod =  (string) environment["owin.RequestMethod"];
            var requestSchema = (string)environment["owin.RequestScheme"];
            var requestPath = (string)environment["owin.RequestPath"];
            var requestQueryString = (string)environment["owin.RequestQueryString"];

            var requestHeaders = (IDictionary<string, string[]>)environment["owin.RequestHeaders"];

            var responseStream = (Stream)environment["owin.ResponseBody"];

            var responseHeaders = (IDictionary<string, string[]>)  environment["owin.ResponseHeaders"];

            responseHeaders["Content-Type"] = new string[] { "application/json " };

            string responseText = "{\"Name\":\"OWIN ąć\"}";
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);

            // return Task.CompletedTask;
            return responseStream.WriteAsync(responseBytes, 0, responseBytes.Length);
        }
    }
}
