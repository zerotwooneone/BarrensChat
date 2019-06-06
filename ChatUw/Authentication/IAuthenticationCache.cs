namespace ChatUw.Authentication
{
    public interface IAuthenticationCache
    {
        AuthModel GetAuthenticationToken();
        void SetAuthenticationToken(AuthModel authModel);
    }
}