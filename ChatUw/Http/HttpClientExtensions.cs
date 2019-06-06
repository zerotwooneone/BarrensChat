using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatUw.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostJson(this HttpClient httpClient, string requestUri, object publishModel)
        {
            var json = JsonConvert.SerializeObject(publishModel);
            var content = new StringContent(json,
                System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(requestUri, content);
            return response;
        }

        public static async Task<HttpResponseMessage<T>> PostJson<T>(this HttpClient httpClient, string requestUri, object publishModel)
        {
            var json = JsonConvert.SerializeObject(publishModel);
            var content = new StringContent(json,
                System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(requestUri, content);
            var obj = response.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())
                : default(T);
            var result = new HttpResponseMessage<T>(response, obj);
            return result;
        }
    }
}