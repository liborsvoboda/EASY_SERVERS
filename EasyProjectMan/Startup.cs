using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Models.Context;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ProjectManagement {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data", "config.txt"));
            services.AddDbContext<ApplicationDbContext>(cfg => { cfg => cfg.UseSqlServer(json) });


            services.Configure<CookiePolicyOptions>(options => {
                // This lambda determines whether user consent for non-essential cookies is needed
                // for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            });

            // setting file size
            services.Configure<FormOptions>(x => {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            // for session
            services.AddMvc(option => {
                option.EnableEndpointRouting = false;
            });
            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddSession(options => {
                options.Cookie.Name = "ProjectManagement";
                options.IdleTimeout = TimeSpan.FromSeconds(300);
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = false;
                options.Cookie.SameSite = SameSiteMode.Unspecified;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            }); ;
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext applicationDbContext) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }


            // Migrate latest database changes during startup
            try {
                applicationDbContext.Database.Migrate();
            } catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Environment.NewLine + ex.Message + Environment.NewLine + "You can Import Database bak File in Installed folder Manually or Allow Create Database for User in Connection string" + Environment.NewLine); 
            }


            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // for session
            app.UseSession();
            app.UseResponseCaching();
            app.Use(async (context, next) => {
                await next(); if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value)) { context.Request.Path = "/"; await next(); }
            });
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=LogIn}/{action=Login}/{id?}");
            });
        }


        //Functions Part
        /// <summary>
        /// Set Imported Certificate from file on Server for Https 
        /// File must has Valid path from Startup Data Path
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 GetSelfSignedCertificateFromFile(string certificateFilename, string password) {
            byte[]? certificate = null;
            try {
                certificate = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data", certificateFilename));
                return new X509Certificate2(certificate, password.Length > 0 ? password : "");
            } catch (Exception Ex) {
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

            using (var rsa = RSA.Create(rsaKeySize)) {
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

                try { //Saving Autogenerate Certificate
                    byte[] exportedData = certificate.Export(X509ContentType.Pfx, password);
                    File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data", "AutoGeneratedCertificate.pfx"), exportedData);
                } catch { }

                return new X509Certificate2(certificate.Export(X509ContentType.Pfx, password), password, X509KeyStorageFlags.Exportable);
            }
        }
    }
}