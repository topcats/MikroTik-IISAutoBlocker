using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;

namespace MikroTik_IISAutoBlocker
{
    public partial class ServiceMain : ServiceBase
    {

        public const string EventLogSourceName = "MikroTik-IISAutoBlocker";

        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Trace.TraceInformation("MikroTik-IISAutoBlocker.ServiceMain.OnStart() V:{0}", Assembly.GetEntryAssembly().GetName().Version);
            EventLog.WriteEntry(EventLogSourceName, $"Service Started {Environment.NewLine}V:{Assembly.GetEntryAssembly().GetName().Version}", EventLogEntryType.Information, 10);

            StartActionProgs();
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry(EventLogSourceName, "Service Stopped", EventLogEntryType.Information, 11);
            Trace.TraceInformation("MikroTik-IISAutoBlocker.ServiceMain.OnStop()");
        }


        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);

            //Fake wait for 10 mins
            System.Threading.Thread.Sleep(600000);

            Console.WriteLine("FINISHED:");
            Console.ReadLine();
            this.OnStop();
        }



        internal void StartActionProgs()
        {

            // Read Config
            string baseFolder = Properties.Settings.Default.IISLogRootPath;
            StringCollection subFolderList = Properties.Settings.Default.IISSubFolder;

            // Validate Config
            if (string.IsNullOrWhiteSpace(baseFolder))
                throw new Exception("Configuration Error: IISLogRootPath is not set in app.config");
            if (subFolderList == null || subFolderList.Count == 0)
                throw new Exception("Configuration Error: IISSubFolder is not set in app.config");


            // Start ActionProgs
            foreach (string subFolder in subFolderList)
            {
                EventLog.WriteEntry(EventLogSourceName, $"Listening to foled: {subFolder}", EventLogEntryType.Information, 21);
                string fullPath = System.IO.Path.Combine(baseFolder, subFolder);
                Trace.TraceInformation("Starting ActionProg for folder: {0}", fullPath);
                new Intelligence.ActionProg(subFolder);
            }

        }
    }
}
