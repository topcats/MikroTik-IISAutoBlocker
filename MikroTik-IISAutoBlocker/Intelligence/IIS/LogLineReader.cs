using MikroTik_IISAutoBlocker.Models;
using System;
using System.Diagnostics;
using System.IO;

namespace MikroTik_IISAutoBlocker.Intelligence.IIS
{
    internal class LogLineReader
    {
        LogFileLastRead _logFileLastRead;


        public LogLineReader(string LogFilePath)
        {
            // Check if already processed this file
            this._logFileLastRead = ActionProg.LogFileLastRead.Find(x => x.LogFileName == LogFilePath) ?? new LogFileLastRead()
            {
                LogFileName = LogFilePath,
                LastReadTime = DateTime.UtcNow
            };

            ReadNewLines();

            SaveLastRead();
        }



        /// <summary>
        /// Read new lines from the log file starting from the last read position and process them. Update the last read position as we go through the lines.
        /// </summary>
        private void ReadNewLines()
        {
            // Read file
            string content = string.Empty;
            using (FileStream stream = File.Open(this._logFileLastRead.LogFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        content += reader.ReadToEnd();
                    }
                }
            }

            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // has old read position,, use that else just get last few lines
            if (this._logFileLastRead.LastReadPosition == 0)
            {
                int linecount = lines.Length;
                this._logFileLastRead.LastReadPosition = (linecount < 10) ? 5 : linecount - 5;
            }

            // Get lines
            string safeSubnet = Properties.Settings.Default.SafeSubnet;
            string line;
            do
            {
                // Read Line
                if (this._logFileLastRead.LastReadPosition >= lines.Length)
                    break;
                line = lines[this._logFileLastRead.LastReadPosition];

                // Convert Line
                var logData = ReadLine(line);
                if (logData == null)
                    break;

                // Process Details
                if (logData.ServerStatus >= 400 &&
                    ((logData.ClientSourceIP == "-" && !logData.ClientIP.StartsWith(safeSubnet)) ||
                    (logData.ClientSourceIP != "-" && !logData.ClientSourceIP.StartsWith(safeSubnet))))
                {
                    // Problem found so block it
                    Trace.TraceInformation("MikroTik-IISAutoBlocker.LogLineReader: {0} {1} {2} {3} {4} {5}", logData.Date, logData.Time, logData.ClientIP, logData.ClientMethod, logData.ClientURIStem, logData.ServerStatus);
                }

                this._logFileLastRead.LastReadPosition++;

            } while (!string.IsNullOrWhiteSpace(line));
        }



        /// <summary>
        /// Save Last Read Position and Time for this file so that next time we can start from there
        /// </summary>
        private void SaveLastRead()
        {
            var existing = ActionProg.LogFileLastRead.Find(x => x.LogFileName == this._logFileLastRead.LogFileName);
            if (existing != null)
            {
                existing.LastReadTime = this._logFileLastRead.LastReadTime;
                existing.LastReadPosition = this._logFileLastRead.LastReadPosition;
            }
            else
            {
                ActionProg.LogFileLastRead.Add(this._logFileLastRead);
            }
        }


        /// <summary>
        /// Read IIS Line and split as needed
        /// </summary>
        /// <param name="line">Raw IIS Log Line</param>
        /// <returns>LogFileLine output</returns>
        public static LogFileLine ReadLine(string line)
        {
            try
            {
                // Split line into parts
                if (string.IsNullOrWhiteSpace(line))
                    return null;
                var parts = line.Split(' ');
                if (parts.Length < 21)
                    return null;
                return new LogFileLine
                {
                    Date = DateTime.Parse(parts[0]),
                    Time = DateTime.Parse(parts[1]),
                    ServerSitename = parts[2],
                    ServerComputername = parts[3],
                    ServerIP = parts[4],
                    ClientMethod = parts[5],
                    ClientURIStem = parts[6],
                    ClientURIQuery = parts[7],
                    ServerPort = int.Parse(parts[8]),
                    ClientUsername = parts[9],
                    ClientIP = parts[10],
                    ClientUserAgent = parts[11],
                    ClientReferer = parts[12],
                    ServerSiteName = parts[13],
                    ServerStatus = int.Parse(parts[14]),
                    ServerSubstatus = int.Parse(parts[15]),
                    ServerWinStatus = int.Parse(parts[16]),
                    BytesSent = int.Parse(parts[17]),
                    BytesReceived = int.Parse(parts[18]),
                    TimeTaken = int.Parse(parts[19]),
                    ClientSoapAction = parts[20],
                    ClientSourceIP = parts[21]
                };
            }
            catch (Exception ex)
            {
                Trace.TraceError("LogLineReader.ReadLine: Error parsing line: {0}. Exception: {1}", line, ex);
                return null;
            }
        }
    }
}
