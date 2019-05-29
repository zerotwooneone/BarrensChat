using System;
using System.Net.Http;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http.Filters;

namespace ChatUw.Http
{
    [Obsolete("this is only meant for localhost testing")]
    public class LocalHostTestHttpClientFactory : HttpClientFactory
    {
        public override HttpClient CreateHttpClient(string bearerToken = null)
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
        private static HttpMessageHandler GetLocalHostSslHack()
        {
            var filter = new HttpBaseProtocolFilter(); // do something with this
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
            var winRtHttpClientHandler = new WinRtHttpClientHandler(filter);
            return winRtHttpClientHandler;
        }
    }
}