using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BackendServer.Auth0;
using BackendServer.Authentication;
using BackendServer.Authorization;
using BackendServer.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackendServer.AppStartup
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppStartupController : ControllerBase
    {
        private readonly IHubRegistrationService _hubRegistrationService;
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IOptionsMonitor<Auth0Config> _auth0ConfigAccessor;

        public AppStartupController(IHubRegistrationService hubRegistrationService,
            IUserAuthenticationService userAuthenticationService,
            IOptionsMonitor<Auth0Config> auth0ConfigAccessor)
        {
            _hubRegistrationService = hubRegistrationService;
            _userAuthenticationService = userAuthenticationService;
            _auth0ConfigAccessor = auth0ConfigAccessor;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ColdRequestModel requestModel)
        {
            if (requestModel?.DeviceInfo == null ||
                string.IsNullOrWhiteSpace(requestModel.DeviceInfo.Handle) ||
                InvalidPlatForm(requestModel.DeviceInfo.Platform)) return BadRequest();

            var reg = await _hubRegistrationService.CreateHubRegistration(requestModel.DeviceInfo.Handle);

            var clientAuthConfigModel = GetClientAuthConfigModel();

            return Ok(new StartupResponseModel
            {
                ClientAuth = clientAuthConfigModel,
                HubRegistration = reg,
                Authentication = null
            });
        }

        [HttpPost]
        [Route("Register")]
        [Authorize(Policy = BarrensChatPolicy.VerifiedEmail)]
        public async Task<IActionResult> PostUser(RegisterUserRequestModel requestModel)
        {
            if (requestModel?.DeviceInfo == null || 
                string.IsNullOrWhiteSpace(requestModel.DeviceInfo.Handle) || 
                InvalidPlatForm(requestModel.DeviceInfo.Platform) || 
                string.IsNullOrWhiteSpace(requestModel.UserInfo?.UserName)) return BadRequest();

            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userAuthenticationService.Add3rdPartyUser(requestModel.UserInfo.UserName, email);
            if (user?.User == null)
            {
                return BadRequest("User could not be created. The username is invalid or is already be in use.");
            }

            var update = requestModel.Registration?.RegistrationId == null
                ? null
                : await _hubRegistrationService.UpdateHubRegistration(new UpdateRegistrationRequestModel
                {
                    Handle = requestModel.DeviceInfo.Handle,
                    Platform = requestModel.DeviceInfo.Platform,
                    RegistrationId = requestModel.Registration.RegistrationId,
                    Username = requestModel.UserInfo.UserName
                });
            HubRegistrationModel reg;
            if (update == null)
            {
                //todo: need to pass tags
                reg = await _hubRegistrationService.CreateHubRegistration(requestModel.DeviceInfo.Handle);
            }
            else
            {
                reg = new HubRegistrationModel
                {
                    RegistrationId = update.RegistrationId
                };
            }

            var clientAuthConfigModel = GetClientAuthConfigModel();

            return Ok(new StartupResponseModel
            {
                ClientAuth = clientAuthConfigModel,
                HubRegistration = reg,
                Authentication = new AuthenticationModel
                {
                    Token = string.Empty
                },
            });
        }

        private static bool InvalidPlatForm(PlatFormId platFormId)
        {
            return (platFormId <= PlatFormId.MicrosoftPushNotificationService &&
                    platFormId > PlatFormId.FirebaseCloudMessaging);
        }

        private ClientAuthConfigModel GetClientAuthConfigModel()
        {
            var auth0 = _auth0ConfigAccessor.CurrentValue;
            var clientAuthConfigModel = new ClientAuthConfigModel
            {
                ClientId = auth0.ClientId,
                Domain = auth0.Domain,
                Scope = auth0.Scope
            };
            return clientAuthConfigModel;
        }
    }
}