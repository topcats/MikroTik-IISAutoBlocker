using System;

namespace MikroTik_IISAutoBlocker.Models
{
    internal class LogFileLastRead
    {
        /// <summary>
        /// Full path of the log file
        /// </summary>
        public string LogFileName { get; set; }

        public long LastReadPosition { get; set; }

        public DateTime LastReadTime { get; set; }
    }
}
