using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Threading;
namespace BFAuctions
{
    internal class Program
    {
        static void StartKestrel()
        {
            Console.WriteLine("Starting....");
            IHost host = CreateHostBuilder(null).Build();
            host.Run();
        }

        static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            DotNetEnv.Env.TraversePath().Load();

            var kestrelThread = new Thread(StartKestrel);
            kestrelThread.Start();

            DBService.InitializePeers();

            MainDispatcher dispatcher = new MainDispatcher();
            


            string command = string.Empty;

            while (!(command == "EX"))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Enter a command: ");
                command = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                bool commandKnown = dispatcher.HandleCommand(command);
                Console.ForegroundColor = ConsoleColor.Blue;
                if (!commandKnown)
                {
                    Console.WriteLine();
                    Console.WriteLine("Command not recognized");
                    Console.WriteLine();
                }
            }
            


        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
       
          .ConfigureWebHostDefaults(webBuilder =>
          {
              webBuilder.ConfigureKestrel(options =>
              {

                  var GRPC_WEB_PORT = DotNetEnv.Env.GetInt("GRPC_WEB_PORT");
                  var GRPC_PORT = DotNetEnv.Env.GetInt("GRPC_PORT");

                  options.ListenAnyIP(GRPC_WEB_PORT, listenOptions => listenOptions.Protocols = HttpProtocols.Http1AndHttp2); //webapi
                  options.ListenAnyIP(GRPC_PORT, listenOptions => listenOptions.Protocols = HttpProtocols.Http2); //grpc
              });

              // start
              webBuilder.UseStartup<Startup>();
 
            
          }).ConfigureLogging(logging =>
          {
              // Example: Remove the console logger
              logging.ClearProviders();
          });

    }
}
