using DaOAuthV2.Constants;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Service;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.IO;

namespace DaOAuthV2.OAuth.Api
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
            services.AddLocalization(options => options.ResourcesPath = ResourceConstant.ResourceFolder);
            services.Configure<AppConfiguration>(Configuration.GetSection("AppConfiguration"));

            var conf = Configuration.GetSection("AppConfiguration").Get<AppConfiguration>();

           services.AddAuthentication(conf.DefaultScheme).AddCookie(conf.DefaultScheme,
               options =>
                {
                    options.DataProtectionProvider = DataProtectionProvider.Create(
                        new DirectoryInfo(conf.DataProtectionProviderDirectory));
                    options.Cookie.Domain = string.Concat(".", conf.AppsDomain);
                    options.LoginPath = "/Account/RedirectToLogin";
                });

            var sp = services.BuildServiceProvider();
            var localizationServiceFactory = sp.GetService<IStringLocalizerFactory>();
            var loggerServiceFactory = sp.GetService<ILoggerFactory>();

            services.AddTransient<IAuthorizeService>(u => new AuthorizeService()
            {
                Configuration = conf,
                RepositoriesFactory = new EfRepositoriesFactory(),
                ConnexionString = Configuration.GetConnectionString("DaOAuthConnexionString"),
                StringLocalizerFactory = localizationServiceFactory,
                Logger = loggerServiceFactory.CreateLogger<UserService>()
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
