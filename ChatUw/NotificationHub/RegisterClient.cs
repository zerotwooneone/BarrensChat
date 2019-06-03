using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChatUw.Http;
using Newtonsoft.Json;

namespace ChatUw.NotificationHub
{
    public class RegisterClient
    {
        private readonly HttpClientFactory _httpClientFactory;
        private readonly Uri _postUri;

        private class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
        }

        public RegisterClient(HttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            var backendUri = new Uri(MagicValues.BackendUrl);
            _postUri = new Uri(backendUri, "/api/register"); ;
        }

        private static DeviceRegistration GetDeviceRegistration(string handle)
        {
            var deviceRegistration = new DeviceRegistration
            {
                Platform = "wns",
                Handle = handle
            };
            return deviceRegistration;
        }

        public async Task<bool> TryUpdateRegistrationAsync(string regId, string handle, string authToken)
        {
            var deviceRegistration = GetDeviceRegistration(handle);
            using (var httpClient = _httpClientFactory.CreateHttpClient(authToken))
            {
                var putUri = new Uri(_postUri, $"/api/register/{regId}");

                string json = JsonConvert.SerializeObject(deviceRegistration);
                var response = await httpClient.PutAsync(putUri, new StringContent(json, Encoding.UTF8, "application/json"));
                return response.StatusCode != HttpStatusCode.Gone;
            }
        }

        public async Task<string> RequestNewRegistrationAsync(string authToken)
        {
            using (var httpClient = _httpClientFactory.CreateHttpClient(authToken))
            {
                var response = await httpClient.PostAsync(_postUri, new StringContent(""));
                if (response.IsSuccessStatusCode)
                {
                    string regId = await response.Content.ReadAsStringAsync();
                    return regId;
                }
                else
                {
                    throw new WebException(response.StatusCode.ToString());
                }
            }
        }


    }
}
