using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.SelfHost;

namespace CryptidRest {
    class Program {
        private const string ServiceAddress = "http://localhost:8080";

        static void Main(string[] args) {
            var config = new HttpSelfHostConfiguration(ServiceAddress);

            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));


            using (HttpSelfHostServer server = new HttpSelfHostServer(config)) {
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

        public static bool IsAdministrator() {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent())) .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
