using DaOAuthV2.Constants;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.OAuth.Api.Filters;
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
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Threading.Tasks;

namespace DaOAuthV2.OAuth.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment CurrentEnvironment { get; set; }

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
                        context.Response.Redirect($"{conf.LoginPageUrl.AbsoluteUri}?returnUrl={conf.OauthApiUrl}{context.RedirectUri.Split("ReturnUrl=")[1]}");                        
                        return Task.CompletedTask;
                    };
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

            services.AddMvc(options =>
                options.Filters.Add(new DaOAuthExceptionFilter(CurrentEnvironment, loggerServiceFactory))).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "DaOAuth OAuth API", Version = "v1" });
                c.IncludeXmlComments(GetXmlCommentsPath());
            });
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "DaOAuth OAuth API");
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
