using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AuthApp
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie()
                    .AddOpenIdConnect(options => {
                        options.ClientId = Configuration["OpenIdSettings:ClientId"];
                        options.ClientSecret = Configuration["OpenIdSettings:ClientSecret"];
                        options.Authority = Configuration["OpenIdSettings:Authority"];
                        options.ResponseType = OpenIdConnectResponseType.Code;
                        options.GetClaimsFromUserInfoEndpoint = true;
                        options.SaveTokens = true;
                        options.Events = new OpenIdConnectEvents()
                        {
                            OnRedirectToIdentityProvider = (context) =>
                            {
                                if (context.Request.Path != "/account/external")
                                {
                                    context.Response.Redirect("/account/login");
                                    context.HandleResponse();
                                }

                                return Task.FromResult(0);
                            }
                        };
                    });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
