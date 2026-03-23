using MikroTik_IISAutoBlocker.Intelligence.IIS;
using MikroTik_IISAutoBlocker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace MikroTik_IISAutoBlocker.Intelligence
{
    internal class ActionProg : IDisposable
    {
        private readonly string _SubFolderName;

        BackgroundWorker bgWorker = new BackgroundWorker();
        private bool disposedValue;

        private LogFileMonitor _logfilemonitor;


        public static List<LogFileLastRead> LogFileLastRead;

        public ActionProg(string subFolderName)
        {
            Trace.TraceInformation("MikroTik-IISAutoBlocker.ActionProg({0})", subFolderName);

            if (ActionProg.LogFileLastRead == null)
            {
                ActionProg.LogFileLastRead = new List<LogFileLastRead>();
            }


            this._SubFolderName = subFolderName;

            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;

            // Assign event handlers
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.ProgressChanged += bgWorker_ProgressChanged;
            bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;

            // Start the BackgroundWorker
            bgWorker.RunWorkerAsync();
        }



        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string baseFolder = Properties.Settings.Default.IISLogRootPath;
            string fullPath = System.IO.Path.Combine(baseFolder, this._SubFolderName);

            this._logfilemonitor = new LogFileMonitor(fullPath);

            while (!e.Cancel)
            {
                // Just Hold the thread for a while to simulate waiting for an event, like a file change
                Thread.Sleep(60000);

                // Check for cancellation
                if (bgWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }



        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update the UI with the operation's progress
            Trace.WriteLine($"MikroTik-IISAutoBlocker.ActionProg.bgWorker_ProgressChanged({this._SubFolderName}) Progress: {e.ProgressPercentage}%");
        }



        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Trace.TraceError("MikroTik-IISAutoBlocker.ActionProg.bgWorker_RunWorkerCompleted({0}) {1}", this._SubFolderName, e.Error.Message);
            }
            else if (e.Cancelled)
            {
                Trace.TraceWarning("MikroTik-IISAutoBlocker.ActionProg.bgWorker_RunWorkerCompleted({0}) Cancelled", this._SubFolderName);
            }
            else
            {
                Trace.TraceInformation("MikroTik-IISAutoBlocker.ActionProg.bgWorker_RunWorkerCompleted({0}) Operation ended", this._SubFolderName);
            }
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    bgWorker.CancelAsync();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
