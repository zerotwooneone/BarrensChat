using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IAuthenticationCache _authenticationCache;
        private readonly IRegistrationCache _registrationCache;
        private readonly Uri _postUri;

        private class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
        }

        public RegisterClient(string backendEndpoint,
            HttpClientFactory httpClientFactory,
            IAuthenticationCache authenticationCache,
            IRegistrationCache registrationCache)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationCache = authenticationCache;
            _registrationCache = registrationCache;
            var backendUri = new Uri(backendEndpoint);
            _postUri = new Uri(backendUri, "/api/register"); ;
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
                _registrationCache.SetRegistrationId(null);
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
            using (var httpClient = _httpClientFactory.CreateHttpClient(_authenticationCache.GetAuthenticationToken()))
            {
                var putUri = new Uri(_postUri, $"/api/register/{regId}");

                string json = JsonConvert.SerializeObject(deviceRegistration);
                var response = await httpClient.PutAsync(putUri, new StringContent(json, Encoding.UTF8, "application/json"));
                return response.StatusCode;
            }
        }

        private async Task<string> RetrieveRegistrationIdOrRequestNewOneAsync()
        {
            if (!_registrationCache.HasRegistrationId())
            {
                using (var httpClient = _httpClientFactory.CreateHttpClient(_authenticationCache.GetAuthenticationToken()))
                {
                    var response = await httpClient.PostAsync(_postUri, new StringContent(""));
                    if (response.IsSuccessStatusCode)
                    {
                        string regId = await response.Content.ReadAsStringAsync();
                        _registrationCache.SetRegistrationId(regId);
                    }
                    else
                    {
                        throw new System.Net.WebException(response.StatusCode.ToString());
                    }
                }
            }
            return _registrationCache.GetRegistrationId();;

        }


    }
}
