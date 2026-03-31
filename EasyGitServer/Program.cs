using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EASYTools.LicenseVerify;
using GitServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;

namespace GitServer  {


    public class Program {

        private static string[] cmdArguments = Environment.GetCommandLineArgs();

        private static string CheckDefaultPort() {
            string port = "5002";
            for (int i = 0; i < cmdArguments.Length; i++) {
                if (cmdArguments[i].Contains("port:")) { port = cmdArguments[i].Split(":").Count() > 1 ? cmdArguments[i].Split(":")[1].Replace("\"", "").Replace("'", "") : port; }
            }
            return port;
        }

        public static string CheckDomain() {
            string domain = "";
            for (int i = 0; i < cmdArguments.Length; i++) {
                if (cmdArguments[i].ToLower().Contains("domain:")) { domain = cmdArguments[i].Split(":").Count() > 1 ? cmdArguments[i].Split(":")[1].Replace("\"", "").Replace("'", "") : domain; }
            }
            return domain;
        }

        private static string CheckPfxFilename() {
            string pfxFilename = "";
            for (int i = 0; i < cmdArguments.Length; i++) {
                if (cmdArguments[i].ToLower().Contains("pfxfilename:")) { pfxFilename = cmdArguments[i].Split(":").Count() > 1 ? cmdArguments[i].Split(":")[1].Replace("\"", "").Replace("'", "") : pfxFilename; }
            }
            return pfxFilename;
        }

        private static string CheckPfxPassword() {
            string pfxPassword = "";
            for (int i = 0; i < cmdArguments.Length; i++) {
                if (cmdArguments[i].ToLower().Contains("pfxpassword:")) { pfxPassword = cmdArguments[i].Split(":").Count() > 1 ? cmdArguments[i].Split(":")[1].Replace("\"", "").Replace("'", "") : pfxPassword; }
            }
            return pfxPassword;
        }

        private static string CheckProtocol() {
            string port = "http";
            for (int i = 0; i < cmdArguments.Length; i++) {
                if (cmdArguments[i].Contains("protocol:")) { port = cmdArguments[i].Split(":").Count() > 1 ? cmdArguments[i].Split(":")[1].Replace("\"", "").Replace("'", "") : port; }
            }
            return port;
        }




        public static void Main(string[] args)
        {
            CheckLicense();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                //CreateHostBuilder(args).UseWindowsService().Build().Run();
                CreateHostBuilder(args).Build().Run();
            }
            else {
                CreateHostBuilder(args).Build().Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>

                Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => {

                    if (CheckProtocol().ToLower() == "https") {
                        webBuilder.ConfigureKestrel(options => {
                            options.AddServerHeader = true;
                            options.ListenAnyIP(int.Parse(CheckDefaultPort()), opt => {
                                opt.Protocols = HttpProtocols.Http1AndHttp2;
                                opt.KestrelServerOptions.AllowAlternateSchemes = true;

                                opt.UseHttps(CheckPfxFilename().Length > 0
                            ? Startup.GetSelfSignedCertificateFromFile(CheckPfxFilename(), CheckPfxPassword())
                                : Startup.GetSelfSignedCertificate(CheckPfxPassword()),
                              httpsOptions => {

                                              httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls | System.Security.Authentication.SslProtocols.Ssl2 | System.Security.Authentication.SslProtocols.Ssl3;
                                              httpsOptions.ClientCertificateMode = ClientCertificateMode.NoCertificate;
                                              httpsOptions.AllowAnyClientCertificate();

                                          });

                                opt.UseConnectionLogging();

                            });
                        });
                    }



                    webBuilder.UseStartup<Startup>().UseWebRoot("wwwroot")
                    .UseUrls($"{CheckProtocol()}://*:{CheckDefaultPort()}");
                });


        private static void CheckLicense() {
            bool licenseStatus = LicenseControlller.VerifyLicense(out LicenseData licenseModel, true);
            if (string.IsNullOrEmpty(licenseModel.Status)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Missing License file in \"Data\" folder");
            }
            Console.WriteLine("License Info: " + System.Text.Json.JsonSerializer.Serialize(licenseModel));
            if (!licenseStatus) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Server will be in 30 second ShutDown"); Thread.Sleep(30 * 1000); Environment.Exit(20);
            }
        }
    }

}
