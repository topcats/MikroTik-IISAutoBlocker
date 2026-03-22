using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroTik_IISAutoBlocker.Intelligence.IIS
{
    public class LogFileMonitor
    {
        private readonly string _logDirectory;

        private readonly FileSystemWatcher _watcher;

        public LogFileMonitor(string logDirectory)
        {
            _logDirectory = logDirectory;
            _watcher = new FileSystemWatcher(_logDirectory)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                Filter = "*.log",
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };
            _watcher.Created += OnChanged;
            _watcher.Changed += OnChanged;
            _watcher.Error += OnError;
        }





        private static void OnChanged(object sender, FileSystemEventArgs e) => Console.WriteLine($"{e.ChangeType}: {e.FullPath}");

        private static void OnError(object sender, ErrorEventArgs e) => Console.WriteLine($"Error: {e.GetException().Message}");
    }
}
