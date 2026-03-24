using MikroTik_IISAutoBlocker.Intelligence.IIS;
using MikroTik_IISAutoBlocker.Intelligence.Router;
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

        private RouterAddressList _routerAddressList;


        public static List<LogFileLastRead> LogFileLastRead;

        public ActionProg(string subFolderName)
        {
            Trace.TraceInformation("MikroTik-IISAutoBlocker.ActionProg({0})", subFolderName);

            if (ActionProg.LogFileLastRead == null)
            {
                ActionProg.LogFileLastRead = new List<LogFileLastRead>();
            }


            this._routerAddressList = new RouterAddressList();
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
            this._logfilemonitor.NewLogLineFound += logfilemonitor_NewLogLine;
            this._logfilemonitor.Listen();

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


        /// <summary>
        /// Log Progress Changes alert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update the UI with the operation's progress
            Trace.WriteLine($"MikroTik-IISAutoBlocker.ActionProg.bgWorker_ProgressChanged({this._SubFolderName}) Progress: {e.ProgressPercentage}% {e.UserState}");
        }



        /// <summary>
        /// The background worker finished??
        /// </summary>
        /// <remarks>It should not get here</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        private void logfilemonitor_NewLogLine(object sender, LogFileLine e)
        {
            // Build Block Item
            var newItem = new RouterAddressItem()
            {
                ListName = Properties.Settings.Default.RouterAddressListName,
                Comment = $"Site: {e.ServerIISname} {e.ServerSiteName} {e.ClientURIStem}",
                Created = e.Time,
                Address = e.ClientIP,
                Timeout = Properties.Settings.Default.RuleExpireTime
            };

            // Add Item
            this._routerAddressList.AddItem(newItem);

            this.bgWorker.ReportProgress(20, $"New Entry: {e.ClientIP} for {e.ServerIISname}");
        }
    }
}
