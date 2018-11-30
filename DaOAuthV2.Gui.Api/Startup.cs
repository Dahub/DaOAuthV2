using DaOAuthV2.Constants;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Gui.Api.Filters;
using DaOAuthV2.Service;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Api
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
                    options.Events.OnRedirectToLogin = (context) =>
                    {
                        context.Response.StatusCode = 401; // API : don't use redirect but unauthorize
                        return Task.CompletedTask;
                    };
                });

            // Build the intermediate service provider
            var sp = services.BuildServiceProvider();
            var localizationServiceFactory = sp.GetService<IStringLocalizerFactory>();
            var loggerServiceFactory = sp.GetService<ILoggerFactory>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddTransient<IUserService>(u => new UserService()
            {
                Configuration = conf,
                RepositoriesFactory = new EfRepositoriesFactory(),
                ConnexionString = Configuration.GetConnectionString("DaOAuthConnexionString"),
                StringLocalizerFactory = localizationServiceFactory,
                Logger = loggerServiceFactory.CreateLogger<UserService>()
            });

            services.AddTransient<IJwtService>(u => new JwtService()
            {
                Configuration = conf,
                RepositoriesFactory = new EfRepositoriesFactory(),
                ConnexionString = Configuration.GetConnectionString("DaOAuthConnexionString"),
                StringLocalizerFactory = localizationServiceFactory,
                Logger = loggerServiceFactory.CreateLogger<JwtService>()
            });

            services.AddMvc(options =>
                options.Filters.Add(new DaOAuthExceptionFilter(CurrentEnvironment, loggerServiceFactory)))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "DaOAuth Gui API", Version = "v1" });
                c.IncludeXmlComments(GetXmlCommentsPath());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IStringLocalizerFactory stringLocalizerFactory, ILoggerFactory loggerFactory)
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

            loggerFactory.AddNLog();
            NLog.LogManager.LoadConfiguration("nlog.config");

            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("fr-FR"),
                new CultureInfo("en-US")
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            app.UseRequestLocalization(options);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "DaOAuth Gui API");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private string GetXmlCommentsPath()
        {
            return Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
        }
    }
}
