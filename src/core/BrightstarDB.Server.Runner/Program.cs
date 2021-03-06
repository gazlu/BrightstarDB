﻿using System;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;

namespace BrightstarDB.Server.Runner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                // Running from the command line
                WriteWelcomeHeader();
                var serviceArgs = new ServiceArgs();
                if (CommandLine.Parser.ParseArgumentsWithUsage(args, serviceArgs))
                {
                    try
                    {
                        var bootstrapper = ServiceBootstrap.GetBootstrapper(serviceArgs);
                        var baseUris = serviceArgs.BaseUris.Select(x => x.EndsWith("/") ? new Uri(x) : new Uri(x+"/")).ToArray();
                        var nancyHost = new NancyHost(bootstrapper, new HostConfiguration{AllowChunkedEncoding=false}, baseUris);
                        nancyHost.Start();
                        Console.ReadLine();
                        nancyHost.Stop();
                    }
                    catch (BootstrapperException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unhandled exception in server: {0}", ex.Message);
                    }
                }
            }
            else
            {
                // Running as a service
                var servicesToRun = new ServiceBase[]
                    {
                        new Service()
                    };
                ServiceBase.Run(servicesToRun);
            }
        }

        private static void WriteWelcomeHeader()
        {
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            var assemVer = fvi.FileMajorPart + "." + fvi.FileMinorPart + "." + fvi.FileBuildPart;

            Console.WriteLine("BrightstarDB REST Server {0}.", assemVer);
            Console.WriteLine(fvi.LegalCopyright);
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine();
        }
    }
}
