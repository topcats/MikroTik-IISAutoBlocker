using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MikroTik_IISAutoBlocker.Intelligence.Router
{
    /// <summary>
    /// MikroTik Router API Control
    /// </summary>
    /// <remarks>https://help.mikrotik.com/docs/spaces/ROS/pages/47579162/REST+API</remarks>
    internal class RouterBase
    {

        public RouterBase()
        {

            // Grab Config
            this._host = Properties.Settings.Default.RouterIP;
            this._username = Properties.Settings.Default.RouterUser;
            this._password = Properties.Settings.Default.RouterPass;
        }



        private readonly string _host;
        private readonly string _username;
        private readonly string _password;



        /// <summary>
        /// Build the URI for the REST API call based on the host and the subpath provided
        /// </summary>
        /// <remarks>The URI will be in the format of "https://{host}/rest/{subpath}"</remarks>
        /// <param name="subpath">api subpath</param>
        private string BuildUri(string subpath)
        {
            return $"{this.BaseUri()}{this.BuildPath(subpath)}";
        }



        /// <summary>
        /// Build Base URI, https and hostname
        /// </summary>
        private string BaseUri()
        {
            return $"https://{this._host}";
        }



        /// <summary>
        /// Build full site path URI
        /// </summary>
        /// <param name="subpath">api subpath</param>
        /// <returns>site path</returns>
        private string BuildPath(string subpath)
        {
            return $"/rest/{subpath}";
        }



        /// <summary>
        /// Approve all Site Server certificates
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslErrors"></param>
        /// <returns>true by default</returns>
        private static bool ServerCertificateCustomValidation(HttpRequestMessage requestMessage, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslErrors)
        {
            //// It is possible to inspect the certificate provided by the server.
            //Console.WriteLine($"Requested URI: {requestMessage.RequestUri}");
            //Console.WriteLine($"Effective date: {certificate?.GetEffectiveDateString()}");
            //Console.WriteLine($"Exp date: {certificate?.GetExpirationDateString()}");
            //Console.WriteLine($"Issuer: {certificate?.Issuer}");
            //Console.WriteLine($"Subject: {certificate?.Subject}");

            //// Based on the custom logic it is possible to decide whether the client considers certificate valid or not
            //Console.WriteLine($"Errors: {sslErrors}");
            //// return  sslErrors == SslPolicyErrors.None;
            return true;
        }



        /// <summary>
        /// Do GET request, and return json string result if successful, otherwise return error message 
        /// </summary>
        /// <param name="path">Subpath with query limits</param>
        /// <returns>Success and result</returns>
        protected (bool, string) DoGet(string path)
        {
            Trace.WriteLine($"Router.RouterBase.DoGet() {path}");

            // Create a handler that bypasses SSL certificate validation
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation
            };

            try
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    // Set base address
                    client.BaseAddress = new Uri(this.BaseUri());

                    // Add Basic Authentication header
                    string authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{this._username}:{this._password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

                    // Optional: Accept JSON
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Send GET request
                    HttpResponseMessage response = client.GetAsync(this.BuildPath(path)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = response.Content.ReadAsStringAsync().Result;
                        return (true, jsonResult);
                    }
                    else
                    {
                        return (false, $"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError("Router.RouterBase.DoGet({0}) HttpRequestException:{1}", path, ex.Message);
                return (false, $"Request error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Router.RouterBase.DoGet({0}) Exception:{1}", path, ex.Message);
                return (false, $"Unexpected error: {ex.Message}");
            }
        }


        /// <summary>
        /// Do Put Request
        /// </summary>
        /// <param name="path">api path</param>
        /// <param name="postdata">post data (json string)</param>
        /// <returns>true if all good</returns>
        internal bool DoPut(string path, string postdata)
        {
            Trace.WriteLine($"Router.RouterBase.DoPut() {path}");

            // Create a handler that bypasses SSL certificate validation
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation
            };

            try
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    // Set base address
                    client.BaseAddress = new Uri(this.BaseUri());

                    // Add Basic Authentication header
                    string authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{this._username}:{this._password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

                    // Optional: Accept JSON
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(postdata, Encoding.UTF8, "application/json");

                    // Send GET request
                    HttpResponseMessage response = client.PutAsync(this.BuildPath(path), content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = response.Content.ReadAsStringAsync().Result;
                        return true;
                    }
                    else
                    {
                        Trace.TraceWarning("Router.RouterBase.DoPut({0}) Failed", path);
                        return false; //$"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Trace.TraceError("Router.RouterBase.DoPut({0}) HttpRequestException:{1}", path, ex.Message);
                return false;//, $"Request error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Router.RouterBase.DoPut({0}) Exception:{1}", path, ex.Message);
                return false;//, $"Unexpected error: {ex.Message}");
            }
        }
    }
}
