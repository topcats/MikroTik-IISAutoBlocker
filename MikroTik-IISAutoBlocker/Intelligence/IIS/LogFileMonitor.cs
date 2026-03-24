using MikroTik_IISAutoBlocker.Models;
using System;
using System.Diagnostics;
using System.IO;

namespace MikroTik_IISAutoBlocker.Intelligence.IIS
{
    internal class LogFileMonitor
    {
        private readonly string _logDirectory;

        private readonly FileSystemWatcher _watcher;

        public LogFileMonitor(string logDirectory)
        {
            Trace.WriteLine($"IIS.LogFileMonitor() {logDirectory}");

            // Setup File System Watcher
            _logDirectory = logDirectory;
            _watcher = new FileSystemWatcher(_logDirectory)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                Filter = "*.log",
                IncludeSubdirectories = false,
                EnableRaisingEvents = false
            };
            _watcher.Created += OnChanged;
            _watcher.Changed += OnChanged;
            _watcher.Error += OnError;
        }



        /// <summary>
        /// Enable Event Listening
        /// </summary>
        public void Listen()
        {
            this._watcher.EnableRaisingEvents = true;
        }


        /// <summary>
        /// Fires when a New Log Line is Found
        /// </summary>
        /// <remarks>Chained from LogLineReader</remarks>
        public event EventHandler<LogFileLine> NewLogLineFound;



        /// <summary>
        /// Fired on a Changed file Detected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Trace.TraceInformation("IIS.LogFileMonitor.OnChanged() {0}: {1}", e.ChangeType, e.FullPath);

            // Read Lines from file and process
            var lfr = new LogLineReader(e.FullPath);
            lfr.NewLogLine += this.NewLogLineFound;
            lfr.Process();
        }



        /// <summary>
        /// Fired on a Error Detected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnError(object sender, ErrorEventArgs e) => Trace.TraceError("IIS.LogFileMonitor.OnError() {0}", e.GetException().Message);
    }
}
