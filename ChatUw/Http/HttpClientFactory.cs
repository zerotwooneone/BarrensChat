using System.Net.Http;

namespace ChatUw.Http
{
    public class HttpClientFactory
    {
        public virtual HttpClient CreateHttpClient(string bearerToken = null)
        {
            var httpClient = new HttpClient();
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                httpClient.SetBearerToken(bearerToken);
            }

            return httpClient;
        }
    }
}