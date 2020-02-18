using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace StreamOutputWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>();
    }
}