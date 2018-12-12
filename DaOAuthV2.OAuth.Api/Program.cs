using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DaOAuthV2.OAuth.Api
{
#pragma warning disable 1591
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
#pragma warning restore 1591
}