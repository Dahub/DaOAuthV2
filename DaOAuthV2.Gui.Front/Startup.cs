using DaOAuthV2.Gui.Front.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Globalization;
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
                    options.LoginPath = "/en/Account/Login";
                });
            
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
               .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
               .AddDataAnnotationsLocalization();

            CultureInfo en = new CultureInfo("en-US");
            en.DateTimeFormat.ShortDatePattern = "dd/MMM/yyyy";
            en.DateTimeFormat.DateSeparator = "/";

            CultureInfo fr = new CultureInfo("fr-FR");
            fr.DateTimeFormat.ShortDatePattern = "dd/MMM/yyyy";
            fr.DateTimeFormat.DateSeparator = "/";

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    en, fr
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new[]{ new DaOAuthRouteDataRequestCultureProvider{
                    IndexOfCulture=1,
                    IndexofUICulture=1
                }};
            });

            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("culture", typeof(LanguageRouteConstraint));
            });


            services.AddSingleton<IConfiguration>(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                       name: "LocalizedDefault",
                       template: "{culture:culture}/{controller=Home}/{action=Dashboard}/{id?}");
                routes.MapRoute(
                        name: "default",
                        template: "{*catchall}",
                        defaults: new { controller = "Home", action = "Dashboard", culture = "en" });
            });
        }
    }
}
