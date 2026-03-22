using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;

namespace MikroTik_IISAutoBlocker
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }


        private void serviceInstallerMain_AfterInstall(object sender, InstallEventArgs e)
        {

            //Create EventLog Sources
            try
            {
                if (!EventLog.SourceExists(ServiceMain.EventLogSourceName))
                    EventLog.CreateEventSource(ServiceMain.EventLogSourceName, "TheOwls");
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("TheOwls", $"MikroTik-IISAutoBlocker Install issue: {ex.ToString()}", EventLogEntryType.Warning);
            }

        }
    }
}
