using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BaGetter.Core;
using BaGetter.Web;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BaGet
{
    public class Program
    {

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((ctx, config) => {
                    var root = Environment.GetEnvironmentVariable("BAGET_CONFIG_ROOT");

                    if (!string.IsNullOrEmpty(root)) {
                        config.SetBasePath(root);
                    } else {
                        if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), " NugetStorage"))) {
                            Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), " NugetStorage"));
                        }
                        config.SetBasePath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), " NugetStorage"));
                    }
                })

                .UseBaGetter(cfg => {

                })

                .ConfigureWebHostDefaults(web => {
                    web.ConfigureKestrel(options => {
                        // Remove the upload limit from Kestrel. If needed, an upload limit can
                        // be enforced by a reverse proxy server, like IIS.
                        options.Limits.MaxRequestBodySize = null;
                        options.ListenAnyIP(8080);
                    });

                    web.UseStartup<Startup>();
                });
        }


        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            if (!host.ValidateStartupOptions())
            {
                return;
            }

            var app = new CommandLineApplication
            {
                Name = "EIC&ESB Nuget Server",
                Description = "A light-weight NuGet service",
            };

            app.HelpOption(inherited: true);

            app.Command("import", import =>
            {
                import.Command("downloads", downloads =>
                {
                    downloads.OnExecuteAsync(async cancellationToken =>
                    {
                        using (var scope = host.Services.CreateScope())
                        {
                            var importer = scope.ServiceProvider.GetRequiredService<DownloadsImporter>();

                            await importer.ImportAsync(cancellationToken);
                        }
                    });
                });
            });

            app.Option("--urls", "The URLs that BaGet should bind to.", CommandOptionType.SingleValue);

            app.OnExecuteAsync(async cancellationToken =>
            {
                await host.RunMigrationsAsync(cancellationToken);
                await host.RunAsync(cancellationToken);
            });

            await app.ExecuteAsync(args);
        }

      
    }
}
