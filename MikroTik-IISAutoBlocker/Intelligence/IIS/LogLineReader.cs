using MikroTik_IISAutoBlocker.Models;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MikroTik_IISAutoBlocker.Intelligence.IIS
{
    internal class LogLineReader
    {
        LogFileLastRead _logFileLastRead;


        /// <summary>
        /// Raised Upon new valid LogLine to be blocked
        /// </summary>
        public event EventHandler<LogFileLine> NewLogLine;


        public LogLineReader(string LogFilePath)
        {
            // Check if already processed this file
            this._logFileLastRead = ActionProg.LogFileLastRead.Find(x => x.LogFileName == LogFilePath) ?? new LogFileLastRead()
            {
                LogFileName = LogFilePath,
                LastReadTime = DateTime.UtcNow
            };
        }



        /// <summary>
        /// Start Process
        /// </summary>
        /// <remarks>ReadNewLine and Update LastRead</remarks>
        public void Process()
        {
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

            // has old read position, use that else just get last few lines
            if (this._logFileLastRead.LastReadPosition == 0)
            {
                int linecount = lines.Length;
                this._logFileLastRead.LastReadPosition = (linecount < 10) ? 5 : linecount - 5;
            }

            // Get lines
            StringCollection safeSubnets = Properties.Settings.Default.SafeSubnets;
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

                // Exclude Safe Subnets
                if ((logData.ClientSourceIP == "-" && safeSubnets.OfType<string>().Any(x => logData.ClientIP.StartsWith(x))) ||
                    (logData.ClientSourceIP != "-" && safeSubnets.OfType<string>().Any(x => logData.ClientSourceIP.StartsWith(x))))
                {
                    // Safe Subnet so skip
                    Trace.WriteLine($"MikroTik-IISAutoBlocker.IIS.LogLineReader: Skip  {logData.Date.ToString("dd/MM/yyyy")} {logData.Time.ToString("HH:mm:ss")} {logData.ClientIP}/{logData.ClientSourceIP} {logData.ClientMethod} {logData.ClientURIStem} {logData.ServerStatus}");
                    this._logFileLastRead.LastReadPosition++;
                    continue;
                }

                // Process Details
                if ((logData.ServerStatus >= 400 && logData.ServerStatus <= 500) || logData.ServerStatus == 503)
                {
                    // Problem found so block it
                    Trace.WriteLine($"MikroTik-IISAutoBlocker.IIS.LogLineReader: BLOCK {logData.Date.ToString("dd/MM/yyyy")} {logData.Time.ToString("HH:mm:ss")} {logData.ClientIP}/{logData.ClientSourceIP} {logData.ClientMethod} {logData.ClientURIStem} {logData.ServerStatus}");
                    NewLogLine.Invoke(this, logData);
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

                if (parts.Length == 13)
                {
                    // HTTPERR Log Format
                    // #Fields: date time c-ip c-port s-ip s-port cs-version cs-method cs-uri sc-status s-siteid s-reason s-queuename
                    return  new LogFileLine
                    {
                        Date = DateTime.Parse(parts[0]),
                        Time = DateTime.Parse(parts[1]),
                        ClientIP = parts[2],
                        ClientPort = GetInt(parts[3]),
                        ServerIP = parts[4],
                        ServerPort = GetInt(parts[5]),
                        // ClientVersion = parts[6],
                        ClientMethod = parts[7],
                        ClientURIStem = parts[8],
                        ServerStatus = GetInt(parts[9]),
                        ServerIISname = parts[10],
                        // ServerReason = parts[11],
                        ServerSiteName = parts[12],
                        ClientSourceIP = "-"
                    };
                }

                if (parts.Length >= 22)
                {
                    // IIS Log Format
                    // #Fields: date time s-sitename s-computername s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) cs-host sc-status sc-substatus sc-win32-status sc-bytes cs-bytes time-taken SOAPAction ClientSourceIP
                    return new LogFileLine
                    {
                        Date = DateTime.Parse(parts[0]),
                        Time = DateTime.Parse(parts[1]),
                        ServerIISname = parts[2],
                        ServerComputername = parts[3],
                        ServerIP = parts[4],
                        ClientMethod = parts[5],
                        ClientURIStem = parts[6],
                        ClientURIQuery = parts[7],
                        ServerPort = GetInt(parts[8]),
                        ClientUsername = parts[9],
                        ClientIP = parts[10],
                        ClientUserAgent = parts[11],
                        ClientReferer = parts[12],
                        ServerSiteName = parts[13],
                        ServerStatus = GetInt(parts[14]),
                        ServerSubstatus = GetInt(parts[15]),
                        ServerWinStatus = GetInt(parts[16]),
                        BytesSent = GetInt(parts[17]),
                        BytesReceived = GetInt(parts[18]),
                        TimeTaken = GetInt(parts[19]),
                        ClientSoapAction = parts[20],
                        ClientSourceIP = parts[21]
                    };
                }

                // If we reach here, the line format is not recognized
                return null;
            }
            catch (Exception ex)
            {
                Trace.TraceError("IIS.LogLineReader.ReadLine: Error parsing line: {0}. Exception: {1}", line, ex);
                return null;
            }
        }



        /// <summary>
        /// Sometime not always an int, but a dash, so return 0
        /// </summary>
        /// <param name="value">string value</param>
        /// <param name="defaultValue"></param>
        /// <returns>Int or 0</returns>
        private static int GetInt(string value, int defaultValue = 0)
        {
            if (int.TryParse(value, out int result))
                return result;
            return defaultValue;
        }
    }
}
