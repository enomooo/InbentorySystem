using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using InbentorySystem.Data;
using System.Net.WebSockets;

namespace InbentorySystem.Tests
{
    public class CustomWebApplicationFactory<TProgram>: WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                var assembly = typeof(TProgram).Assembly;
                var basePath = Path.GetDirectoryName(assembly.Location);

                var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Testing.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

                conf.AddConfiguration(config);
            });

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IDbConnectionFactory));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var serviceProvider = services.BuildServiceProvider();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var testConnectionString = configuration.GetConnectionString("DefaultConnection");

                services.AddSingleton<IDbConnectionFactory>(
                    sp => new NpgsqlConnectionFactory(testConnectionString));
            });
            builder.UseEnvironment("Testing");
        }
    }
}
