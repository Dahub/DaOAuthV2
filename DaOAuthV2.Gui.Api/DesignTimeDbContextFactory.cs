using DaOAuthV2.Dal.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DaOAuthV2.Gui.Api
{
    /// <summary>
    /// Used for add migrations
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DaOAuthContext>
    {
        /// <summary>
        /// Create DB context
        /// </summary>
        /// <param name="args"></param>
        /// <returns>DB context</returns>
        public DaOAuthContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<DaOAuthContext>();
            var connectionString = configuration.GetConnectionString("DaOAuthConnexionString");
            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("DaOAuthV2.Gui.Api"));
            return new DaOAuthContext(builder.Options);
        }
    }
}
