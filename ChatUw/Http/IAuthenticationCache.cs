namespace ChatUw.Http
{
    public interface IAuthenticationCache
    {
        string GetAuthenticationToken();
        void SetAuthenticationToken(string token);
    }
}