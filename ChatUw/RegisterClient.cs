using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Net;
using System.Net.Http;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http.Filters;
using ChatUw.Http;
using Newtonsoft.Json;

namespace ChatUw
{
    class RegisterClient
    {
        private string POST_URL;

        private class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
        }

        public RegisterClient(string backendEndpoint)
        {
            POST_URL = backendEndpoint + "/api/register";
        }

        public async Task RegisterAsync(string handle, IEnumerable<string> tags)
        {
            var regId = await RetrieveRegistrationIdOrRequestNewOneAsync();

            var deviceRegistration = new DeviceRegistration
            {
                Platform = "wns",
                Handle = handle,
                Tags = tags.ToArray<string>()
            };

            var statusCode = await UpdateRegistrationAsync(regId, deviceRegistration);

            if (statusCode == HttpStatusCode.Gone)
            {
                // regId is expired, deleting from local storage & recreating
                var settings = ApplicationData.Current.LocalSettings.Values;
                settings.Remove("__NHRegistrationId");
                regId = await RetrieveRegistrationIdOrRequestNewOneAsync();
                statusCode = await UpdateRegistrationAsync(regId, deviceRegistration);
            }

            if (statusCode != HttpStatusCode.Accepted && statusCode != HttpStatusCode.OK)
            {
                // log or throw
                throw new System.Net.WebException(statusCode.ToString());
            }
        }

        private async Task<HttpStatusCode> UpdateRegistrationAsync(string regId, DeviceRegistration deviceRegistration)
        {
            var settings = ApplicationData.Current.LocalSettings.Values;
            using (var httpClient = CreateHttpClient((string)settings["AuthenticationToken"]))
            {
                var putUri = POST_URL + "/" + regId;

                string json = JsonConvert.SerializeObject(deviceRegistration);
                var response = await httpClient.PutAsync(putUri, new StringContent(json, Encoding.UTF8, "application/json"));
                return response.StatusCode;
            }
        }

        private async Task<string> RetrieveRegistrationIdOrRequestNewOneAsync()
        {
            var settings = ApplicationData.Current.LocalSettings.Values;
            if (!settings.ContainsKey("__NHRegistrationId"))
            {
                using (var httpClient = CreateHttpClient((string)settings["AuthenticationToken"]))
                {
                    var response = await httpClient.PostAsync(POST_URL, new StringContent(""));
                    if (response.IsSuccessStatusCode)
                    {
                        string regId = await response.Content.ReadAsStringAsync();
                        settings["__NHRegistrationId"] = regId;
                    }
                    else
                    {
                        throw new System.Net.WebException(response.StatusCode.ToString());
                    }
                }
            }
            return (string)settings["__NHRegistrationId"];

        }

        private static HttpClient CreateHttpClient(string bearerToken = null)
        {
            var httpMessageHandler = GetLocalHostSslHack();
            var httpClient = new HttpClient(httpMessageHandler);
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                httpClient.SetBearerToken(bearerToken);
            }

            return httpClient;
        }

        
        /// <summary>
        /// This should not be used in production code
        /// </summary>
        /// <returns></returns>
        [Obsolete("this is only meant for localhost testing")]
        private static HttpMessageHandler GetLocalHostSslHack()
        {
            var filter = new HttpBaseProtocolFilter(); // do something with this
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
            var winRtHttpClientHandler = new WinRtHttpClientHandler(filter);
            return winRtHttpClientHandler;
        }
    }
}
