using MikroTik_IISAutoBlocker.Models;
using System;

namespace MikroTik_IISAutoBlocker.Intelligence.IIS
{
    internal static class LogLineReader
    {

        /// <summary>
        /// Read IIS Line and split as needed
        /// </summary>
        /// <param name="line">Raw IIS Log Line</param>
        /// <returns>LogFileLine output</returns>
        public static LogFileLine ReadLine(string line)
        {
            var parts = line.Split(' ');
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
    }
}
