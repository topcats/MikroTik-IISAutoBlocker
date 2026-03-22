using System;

namespace MikroTik_IISAutoBlocker.Models
{
    internal class LogFileLine
    {
        // #Fields: date time s-sitename s-computername s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) cs-host sc-status sc-substatus sc-win32-status sc-bytes cs-bytes time-taken SOAPAction ClientSourceIP

        /// <summary>
        /// date - date in the format YYYY-MM-DD
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// time - time in the format HH:MM:SS (GMT)
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// s-sitename - IIS Site name
        /// </summary>
        public string ServerSitename { get; set; }

        /// <summary>
        /// s-computername - Host Server name
        /// </summary>
        public string ServerComputername { get; set; }

        /// <summary>
        /// s-ip - Server IP address
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// cs-method - HTTP method used by the client (e.g., GET, POST, etc.)
        /// </summary>
        public string ClientMethod { get; set; }

        /// <summary>
        /// cs-uri-stem - URL path requested by the client, excluding the query string (e.g., /index.html)
        /// </summary>
        public string ClientURIStem { get; set; }

        /// <summary>
        /// cs-uri-query - Query string portion of the URL requested by the client (e.g., id=123&name=abc)
        /// </summary>
        public string ClientURIQuery { get; set; }

        /// <summary>
        /// s-port - Server port number that received the request (e.g., 80 for HTTP, 443 for HTTPS)
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// cs-username - Authenticated username if the request was authenticated, otherwise a hyphen (-)
        /// </summary>
        public string ClientUsername { get; set; }

        /// <summary>
        /// c-ip - Client IP address that made the request (might be a proxy)
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// cs(User-Agent) - User-Agent string sent by the client, which typically includes information about the client's browser, operating system, and device type
        /// </summary>
        public string ClientUserAgent { get; set; }

        /// <summary>
        /// cs(Referer) - URL of the referring page that linked to the requested resource, if available. 
        /// </summary>
        public string ClientReferer { get; set; }

        /// <summary>
        /// cs-host - Host header sent by the client, which indicates the domain name of the server being accessed (e.g., www.example.com)
        /// </summary>
        public string ServerSiteName { get; set; }

        /// <summary>
        /// sc-status - HTTP status code returned by the server in response to the client's request (e.g., 200 for success, 404 for not found, etc.)
        /// </summary>
        /// <remarks>https://learn.microsoft.com/en-us/troubleshoot/developer/webapps/iis/health-diagnostic-performance/http-status-code</remarks>
        public int ServerStatus { get; set; }

        /// <summary>
        /// sc-substatus - Substatus code that provides additional information about the HTTP status code returned by the server (e.g., 0 for no substatus, 1 for file not found, etc.)
        /// </summary>
        public int ServerSubstatus { get; set; }

        /// <summary>
        /// sc-win32-status - Windows status code that provides additional information about the request processing on the server (e.g., 0 for success, 2 for file not found, etc.)
        /// </summary>
        public int ServerWinStatus { get; set; }

        /// <summary>
        /// sc-bytes - Number of bytes sent by the server to the client in response to the request (excluding HTTP headers)
        /// </summary>
        public int BytesSent { get; set; }

        /// <summary>
        /// cs-bytes - Number of bytes received by the server from the client in the request (excluding HTTP headers)
        /// </summary>
        public int BytesReceived { get; set; }

        /// <summary>
        /// time-taken - Time taken by the server to process the request and send the response back to the client, measured in milliseconds
        /// </summary>
        public int TimeTaken { get; set; }

        /// <summary>
        /// SOAPAction - The value of the SOAPAction HTTP header sent by the client, which is used in SOAP-based web services to indicate the intent of the SOAP request. This field is typically used when the IIS server is hosting SOAP web services.
        /// </summary>
        public string ClientSoapAction { get; set; }

        /// <summary>
        /// ClientSourceIP - The original client IP address that made the request, which may be different from the c-ip field if the request passed through a proxy or load balancer. This field is typically populated when the IIS server is configured to log the X-Forwarded-For header, which is commonly used to identify the original client IP address in scenarios where requests are forwarded by proxies or load balancers.
        /// </summary>
        public string ClientSourceIP { get; set; }

    }
}
