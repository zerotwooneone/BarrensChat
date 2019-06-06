using System.Net.Http;

namespace ChatUw.Http
{
    public class HttpResponseMessage<T>
    {
        public HttpResponseMessage(HttpResponseMessage message, T o)
        {
            Message = message;
            Object = o;
        }

        public HttpResponseMessage Message { get; }
        public T Object { get; }
    }
}