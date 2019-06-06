﻿using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Auth0.OidcClient;
using ChatUw.Providers;
using IdentityModel.OidcClient;

namespace ChatUw.Authentication
{
    public class JwtAuthService : IAuthenticationService
    {
        private readonly CurrentDateTimeProvider _currentDateTimeProvider;

        public JwtAuthService(CurrentDateTimeProvider currentDateTimeProvider)
        {
            _currentDateTimeProvider = currentDateTimeProvider;
        }
        public AuthModel GetValidAuthModel()
        {
            throw new System.NotImplementedException();
        }

        public AuthModel SetAuthModel(string token)
        {
            var authModel = new AuthModel{Token = token};
            throw new System.NotImplementedException();
        }

        public async Task<LoginModel> Get3rdPartyAuth()
        {
            var loginResult = await GetLoginResult();

            var token = loginResult.IsError ? null : loginResult.IdentityToken;
            var result = new LoginModel { Token = token };
            return result;
        }
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