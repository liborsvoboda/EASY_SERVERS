using GitServer.Services;
using GitServer.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using GitServer.Handlers;
using GitServer.ApplicationCore.Interfaces;
using GitServer.ApplicationCore.Models;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using GitServer.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using GitServer;

namespace GitServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data", "config.txt"));
            services.AddDbContext<GitServerContext>(options => options.UseSqlServer(json));

            //var connectionType = Configuration.GetConnectionString("ConnectionType");
            //string connectionString = Configuration.GetConnectionString("DefaultConnection");
            //switch (connectionType)
            //{
            //    case "Sqlite":
            //        services.AddDbContext<GitServerContext>(options => options.UseSqlite(connectionString)); break;
            //    case "MSSQL":
            //        services.AddDbContext<GitServerContext>(options => options.UseSqlServer(connectionString)); 
            //    break;
            //    case "MySQL":
            //        services.AddDbContext<GitServerContext>(options => options.UseMySQL(connectionString)); break;
            //    default:
            //        services.AddDbContext<GitServerContext>(options => options.UseSqlite(connectionString)); break;
            //}
            // Add framework services.

            services.AddMvc(option => {
                option.EnableEndpointRouting = false;
            }).AddViewComponentsAsServices().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddOptions();
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options => {
                options.AccessDeniedPath = "/User/Login";
                options.LoginPath = "/User/Login";
            }).AddBasic();

            services.Configure<GitSettings>(options => {
                options.BasePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                options.GitPath = "GitStorage";
            });
            // Add git services
            services.AddTransient<GitRepositoryService>();
            services.AddTransient<GitFileService>();
            services.AddTransient<IGitDbRepository<User>, GitDbRepository<User>>();
            services.AddTransient<IGitDbRepository<Repository>, GitDbRepository<Repository>>();

            services.AddEndpointsApiExplorer().AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, GitServerContext gitServerContext) {
            InitializeGitServerDatabase(app.ApplicationServices);

            try {
                gitServerContext.Database.Migrate();
            } catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Environment.NewLine}{ex.Message}{Environment.NewLine}You can Import Database bak File in Installed folder Manually or Allow Create Database for User in Connection string{Environment.NewLine}");
            }


            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/error");
            }


            app.UseAuthentication();
            app.UseStaticFiles();


            app.UseMvc(routes => { 
                RouteConfig.RegisterRoutes(routes);
               
            });
            app.UseMvcWithDefaultRoute();
        }
        private void InitializeGitServerDatabase(IServiceProvider serviceProvider)
        {
            using (IServiceScope serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                GitServerContext db = serviceScope.ServiceProvider.GetService<GitServerContext>();
                db.Database.EnsureCreated();
                if (db.Users.Count() == 0)
                {
                    //db.SaveChanges();
                }
            }
        }
		
		
		//Functions Part
        /// <summary>
        /// Set Imported Certificate from file on Server for Https 
        /// File must has Valid path from Startup Data Path
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 GetSelfSignedCertificateFromFile(string certificateFilename, string password) {
            byte[]? certificate = null;
            try
            {
                certificate = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data", certificateFilename));
                return new X509Certificate2(certificate, password.Length > 0 ? password : "");
            } catch (Exception Ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Environment.NewLine + Ex.Message + Environment.NewLine);
            }
            return GetSelfSignedCertificate(certificateFilename);
        }

        /// <summary>
        /// Gets the self signed certificate For API Security HTTPS.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static X509Certificate2 GetSelfSignedCertificate(string password) {
            var commonName = Program.CheckDomain().Length > 0 ? Program.CheckDomain() : "localhost";
            var rsaKeySize = 2048;
            var years = 10;
            var hashAlgorithm = HashAlgorithmName.SHA256;

            using (var rsa = RSA.Create(rsaKeySize))
            {
                var request = new CertificateRequest($"cn={commonName}", rsa, hashAlgorithm, RSASignaturePadding.Pkcs1);

                SubjectAlternativeNameBuilder subjectAlternativeNameBuilder = new();
                subjectAlternativeNameBuilder.AddDnsName(Assembly.GetExecutingAssembly().GetName().FullName);

                X509BasicConstraintsExtension extension = new();

                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyCertSign, false)
                );

                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                    new OidCollection { new Oid("1.3.6.1.5.5.7.3.1"), new Oid("1.3.6.1.5.5.7.3.2") }, false)
                );

                var notAfter = DateTimeOffset.Now.AddYears(years);
                var certificate = request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), notAfter);
                if (OperatingSystem.IsWindows()) { certificate.FriendlyName = Assembly.GetExecutingAssembly().GetName().FullName; }

                try
                { //Saving Autogenerate Certificate
                    byte[] exportedData = certificate.Export(X509ContentType.Pfx, password);
                    File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data", "AutoGeneratedCertificate.pfx"), exportedData);
                } catch { }

                return new X509Certificate2(certificate.Export(X509ContentType.Pfx, password), password, X509KeyStorageFlags.Exportable);
            }
        }
    }
}
