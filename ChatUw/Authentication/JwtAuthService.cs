using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Auth0.OidcClient;
using ChatUw.Providers;
using IdentityModel.OidcClient;

namespace ChatUw.Authentication
{
    public class JwtAuthService : IAuthenticationService
    {
        private readonly CurrentDateTimeProvider _currentDateTimeProvider;
        private readonly IAuthenticationCache _authenticationCache;

        public JwtAuthService(CurrentDateTimeProvider currentDateTimeProvider,
            IAuthenticationCache authenticationCache)
        {
            _currentDateTimeProvider = currentDateTimeProvider;
            _authenticationCache = authenticationCache;
        }
        public AuthModel GetValidAuthModel()
        {
            var auth = _authenticationCache.GetAuthenticationToken();
            var result = auth == null
                ? null
                : IsExpired(auth.Token)
                    ? null
                    : auth;
            return result;
        }

        public AuthModel SetAuthModel(string token, string username)
        {
            var authModel = new AuthModel{Token = token, Username = username};
            _authenticationCache.SetAuthenticationToken(authModel);
            return authModel;
        }

        public async Task<LoginModel> Get3rdPartyAuth()
        {
            var loginResult = await GetLoginResult();

            var token = loginResult.IsError ? null : loginResult.IdentityToken;
            var result = new LoginModel { Token = token, Username = DummyUserName };
            return result;
        }

        /// <summary>
        /// This is a placeholder till we get the username from the api during auth
        /// </summary>
        public const string DummyUserName = "DummyUserName";

        private bool IsExpired(string authenticationToken)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadToken(authenticationToken);
            var exp = jwtToken.ValidTo;
            var expired = exp < _currentDateTimeProvider.GetCurrentDateTime().ToUniversalTime();
            return expired;
        }

        private async Task<LoginResult> GetLoginResult()
        {
            var client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = MagicValues.Auth0Domain,
                ClientId = MagicValues.Auth0ClientId,
                Scope = "openid email profile"
            });

            var loginResult = await client.LoginAsync();
            return loginResult;
        }
    }
}