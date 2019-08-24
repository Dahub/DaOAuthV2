using DaOAuthV2.ApiTools;
using DaOAuthV2.Dal.EF;
using DaOAuthV2.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DaOAuthV2.Gui.Api
{
    public class TestStartup : Startup
    {
        public const string TestDataBaseName = "inMemoryDatabase";
        public const string TestEnvironnementName = "test";

        public TestStartup(IConfiguration configuration, IHostingEnvironment env) : base(configuration, env)
        {
        }

        protected override DbContextOptions BuildDbContextOptions()
        {
            return new DbContextOptionsBuilder<DaOAuthContext>()
                     .UseInMemoryDatabase(databaseName: TestDataBaseName)
                     .Options;
        }

        protected override void UseSwagger(IServiceCollection services)
        {
            // do nothing
        }

        protected override void AddSwagger(IApplicationBuilder app)
        {
            // do nothing
        }

        protected override void AddLogger(ILoggerFactory loggerFactory)
        {
            // do nothing
        }

        protected override void BuildAuthentification(IServiceCollection services, AppConfiguration conf)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test Scheme";
                options.DefaultChallengeScheme = "Test Scheme";
            }).AddTestAuth(o => { });
        }
    }
}
