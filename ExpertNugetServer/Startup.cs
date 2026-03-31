using System;
using BaGetter;
using BaGetter.Core;
using BaGetter.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BaGet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<CorsOptions>, ConfigureBaGetterOptions>();
            services.AddTransient<IConfigureOptions<FormOptions>, ConfigureBaGetterOptions>();
            services.AddTransient<IConfigureOptions<ForwardedHeadersOptions>, ConfigureBaGetterOptions>();
            //services.AddTransient<IConfigureOptions<IISServerOptions>, ConfigureBaGetterOptions>();
            services.AddTransient<IValidateOptions<BaGetterOptions>, ConfigureBaGetterOptions>();



            //services.AddBaGetterOptions<IISServerOptions>(nameof(IISServerOptions));
            services.AddBaGetterWebApplication(ConfigureBaGetApplication);

            
            services.AddDbContext<BaGetter.Database.Sqlite.SqliteContext>(cfg=> {});


            // You can swap between implementations of subsystems like storage and search using BaGet's configuration.
            // Each subsystem's implementation has a provider that reads the configuration to determine if it should be
            // activated. BaGet will run through all its providers until it finds one that is active.
            services.AddScoped(DependencyInjectionExtensions.GetServiceFromProviders<IContext>);
            services.AddTransient(DependencyInjectionExtensions.GetServiceFromProviders<IStorageService>);
            services.AddTransient(DependencyInjectionExtensions.GetServiceFromProviders<IPackageDatabase>);
            services.AddTransient(DependencyInjectionExtensions.GetServiceFromProviders<ISearchService>);
            services.AddTransient(DependencyInjectionExtensions.GetServiceFromProviders<ISearchIndexer>);

            services.AddSingleton<IConfigureOptions<MvcRazorRuntimeCompilationOptions>, ConfigureRazorRuntimeCompilation>();

          
            services.AddCors();
        }

        private void ConfigureBaGetApplication(BaGetterApplication app)
        {
            // Add database providers.
            //app.AddAzureTableDatabase();
            //app.AddMySqlDatabase();
            //app.AddPostgreSqlDatabase();
            app.AddSqliteDatabase(cfg => { cfg.ConnectionString = "Data Source=baget.db"; cfg.Type = "Sqlite"; });
            app.AddSqlServerDatabase();

            // Add storage providers.
            app.AddFileStorage();
            //app.AddAliyunOssStorage();
            //app.AddAwsS3Storage();
            //app.AddAzureBlobStorage();
            //app.AddGoogleCloudStorage();

            // Add search providers.
            //app.AddAzureSearch();
            app.AddNullSearch();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var options = Configuration.Get<BaGetterOptions>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }

            app.UseForwardedHeaders();
            app.UsePathBase(options.PathBase);

            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors(ConfigureBaGetterOptions.CorsPolicy);
            app.UseOperationCancelledMiddleware();

            app.UseEndpoints(endpoints =>
            {
                var baget = new BaGetterEndpointBuilder();

                baget.MapEndpoints(endpoints);
            });
        }
    }
}
