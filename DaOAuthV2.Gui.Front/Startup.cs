﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace DaOAuthV2.Gui.Front
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        private IHostingEnvironment CurrentEnvironment { get; set; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FrontConfiguration>(Configuration.GetSection("FrontConfiguration"));

            var conf = Configuration.GetSection("FrontConfiguration").Get<FrontConfiguration>();

            services.AddAuthentication(conf.DefaultScheme).AddCookie(conf.DefaultScheme,
                options =>
                {
                    options.DataProtectionProvider = DataProtectionProvider.Create(
                        new DirectoryInfo(conf.DataProtectionProviderDirectory));
                    options.Cookie.Domain = string.Concat(".", conf.AppsDomain);
                    options.LoginPath = "/Account/Login";
                });

            services.AddMvc();
            services.AddSingleton<IConfiguration>(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
