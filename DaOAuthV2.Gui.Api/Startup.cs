using DaOAuthV2.Dal.EF;
using DaOAuthV2.Service;
using DaOAuthV2.Service.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

namespace DaOAuthV2.Gui.Api
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
            services.Configure<AppConfiguration>(Configuration.GetSection("AppConfiguration"));

            services.AddTransient<IUserService>(u => new UserService()
            {
                Configuration = Configuration.GetSection("AppConfiguration").Get<AppConfiguration>(),
                Factory = new EfRepositoriesFactory(),
                ConnexionString = Configuration.GetConnectionString("DaOAuthConnexionString")
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "DaOAuth Gui API", Version = "v1" });
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
