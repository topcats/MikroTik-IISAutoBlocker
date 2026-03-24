using System;
using System.ServiceProcess;

namespace MikroTik_IISAutoBlocker
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            if (Environment.UserInteractive)
            {
                Console.WriteLine("DEBUG MODE");

                ServiceMain service1 = new ServiceMain();
                service1.TestStartupAndStop(null);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                    {
                    new ServiceMain()
                    };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
