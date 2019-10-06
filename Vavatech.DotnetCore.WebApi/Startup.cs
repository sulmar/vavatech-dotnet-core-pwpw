using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vavatech.DotnetCore.FakeRepositories;
using Vavatech.DotnetCore.Fakers;
using Vavatech.DotnetCore.IRepositories;
using Vavatech.DotnetCore.WebApi.Handlers;

namespace Vavatech.DotnetCore.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICustomerRepository, FakeCustomerRepository>();
            services.AddSingleton<CustomerFaker>();
            services.AddSingleton<AddressFaker>();
            services.AddSingleton<ISenderService, MockSmsService>();
            services.AddSingleton<ISenderService, MockFacebookService>();

             services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null)
               ;

            // services.AddAuthorization(options => options
            //     .AddPolicy("RequireDeveloperOnly", 
            //         policy => policy.RequireRole("Developer", "Trainer")));
                
            services.AddAuthorization(options => 
            {
                options
                .AddPolicy("RequireDeveloperOnly", 
                    policy => policy.RequireRole("Developer", "Trainer"));

                options.AddPolicy("HasEmail", 
                    policy => policy.RequireClaim(System.Security.Claims.ClaimTypes.Email));

                options.AddPolicy("MinAge", policy =>
                    policy.Requirements.Add(new MinAgeRequirement(18)));
            });



            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
