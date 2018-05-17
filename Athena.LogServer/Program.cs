using System;
using System.Configuration.Install;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace Athena.LogServer
{
    static class Program
    {
        #region Constants

        /// <summary>
        /// The one and only service name.
        /// </summary>
        public const string ServiceName = "Athena.LogServer";

        #endregion

        #region Entry Point

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //check if executed from command line
            if (Environment.UserInteractive)
            {
                //check parameters
                if (Environment.GetCommandLineArgs().Contains("--install"))
                {
                    InstallService();
                }
                else if (Environment.GetCommandLineArgs().Contains("--uninstall"))
                {
                    UnistallService();
                }
                else
                {
                    //print help message on console
                    Console.WriteLine("Usage:");
                    Console.WriteLine("Athena.LogServer.exe options");
                    Console.WriteLine("Available options:");
                    Console.WriteLine("\t--install: install the service if not installed");
                    Console.WriteLine("\t--uninstall: uninstall the service if installed");
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new LogServer()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs the service installation.
        /// </summary>
        private static void InstallService()
        {
            //check if already installed
            if (ServiceController.GetServices().Any(s => s.ServiceName == ServiceName))
            {
                return;
            }
            //install the service
            ManagedInstallerClass.InstallHelper(new[] { Environment.GetCommandLineArgs().First() });
            //start it
            new ServiceController(ServiceName).Start();
        }
        /// <summary>
        /// Perform the service uninstallation.
        /// </summary>
        private static void UnistallService()
        {
            //check if already installed
            if (ServiceController.GetServices().Any(s => s.ServiceName == ServiceName))
            {
                //stop the service if running
                ServiceController service = new ServiceController(ServiceName);
                if (service.Status != ServiceControllerStatus.Running)
                {
                    service.Stop();
                }
                //uninstall the service
                ManagedInstallerClass.InstallHelper(new[] { "/u", Environment.GetCommandLineArgs().First() });
            }
        }

        #endregion
    }
}
