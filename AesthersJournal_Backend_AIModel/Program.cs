using DotNetEnv; // Add this line
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GeminiAIChatTherapist
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load(); // Load .env file here
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
