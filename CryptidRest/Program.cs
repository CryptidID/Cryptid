using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.SelfHost;

namespace CryptidRest {
    internal class Program {
        /// <summary>
        /// The address to run the service on
        /// </summary>
        private const string ServiceAddress = "http://localhost:8080";

        /// <summary>
        /// The program main method
        /// </summary>
        /// <param name="args">The program arguments</param>
        private static void Main(string[] args) {
            var config = new HttpSelfHostConfiguration(ServiceAddress);

            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));


            using (var server = new HttpSelfHostServer(config)) {
                try {
                    server.OpenAsync().Wait();
                    Console.WriteLine("Service running on " + ServiceAddress + ". Press enter to quit.");
                    Console.ReadLine();
                }
                catch (Exception e) {
                    if (!IsAdministrator()) {
                        Console.WriteLine("Please restart as admin.");
                        Debug.WriteLine("Restart Visual Studio as admin");
                    }
                    else {
                        Console.WriteLine("Server failed to start.");
                    }
                    Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// Checks if the program is being run as administrator
        /// </summary>
        /// <returns></returns>
        public static bool IsAdministrator() {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}