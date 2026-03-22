using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MikroTik_IISAutoBlocker.Intelligence
{
    internal class ActionProg : IDisposable
    {
        private readonly string _SubFolderName;

        BackgroundWorker bgWorker = new BackgroundWorker();
        private bool disposedValue;

        public ActionProg(string subFolderName)
        {
            Trace.TraceInformation("MikroTik-IISAutoBlocker.ActionProg({0})", subFolderName);

            _SubFolderName = subFolderName;

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
            // Here, insert the code for the long-running operation, like downloading a file
            using (var client = new WebClient())
            {
                // Simulate progress reporting
                for (int i = 0; i <= 100; i += 20)
                {
                    if (bgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    bgWorker.ReportProgress(i);
                    Thread.Sleep(4000); // Simulates a task
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
