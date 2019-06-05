using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using BackendServer.AppStartup;
using BackendServer.Auth0;
using BackendServer.Authentication;
using BackendServer.Register;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BackendServer.Tests.AppStartup
{
    [TestClass]
    public class AppStartupControllerTests
    {
        private MockRepository mockRepository;

        private Mock<IHubRegistrationService> mockHubRegistrationService;
        private Mock<IUserAuthenticationService> mockUserAuthenticationService;
        private Mock<IOptionsMonitor<Auth0Config>> _auto0ConfigProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockHubRegistrationService = this.mockRepository.Create<IHubRegistrationService>();
            this.mockUserAuthenticationService = this.mockRepository.Create<IUserAuthenticationService>();
            this._auto0ConfigProvider = this.mockRepository.Create<IOptionsMonitor<Auth0Config>>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.mockRepository.VerifyAll();
        }

        private AppStartupController CreateAppStartupController()
        {
            return new AppStartupController(
                this.mockHubRegistrationService.Object,
                this.mockUserAuthenticationService.Object,
                this._auto0ConfigProvider.Object);
        }

        [TestMethod]
        public async Task Post_NoRequest_BadRequest()
        {
            // Arrange
            var unitUnderTest = this.CreateAppStartupController();
            ColdRequestModel coldRequestModel = null;

            // Act
            var result = (await unitUnderTest.Post(coldRequestModel)) as IStatusCodeActionResult;

            // Assert
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result?.StatusCode);
        }

        [TestMethod]
        public async Task Post_NewInstall_Ok()
        {
            // Arrange
            var unitUnderTest = this.CreateAppStartupController();
            ColdRequestModel coldRequestModel = new ColdRequestModel
            {
                DeviceInfo = new DeviceRegisterModel
                {
                    Handle = "some handle",
                    Platform = PlatFormId.WindowsPushNotificationService
                }
            };
            mockHubRegistrationService
                .Setup(hrs => hrs.CreateHubRegistration(It.IsAny<string>()))
                .ReturnsAsync(new HubRegistrationModel { RegistrationId = "RegistrationId" });
            _auto0ConfigProvider
                .SetupGet(acp => acp.CurrentValue)
                .Returns(new Auth0Config());

            // Act
            var result = (await unitUnderTest.Post(coldRequestModel)) as IStatusCodeActionResult;

            // Assert
            Assert.AreEqual((int)HttpStatusCode.OK, result?.StatusCode);
        }

        [TestMethod]
        public async Task Post_NewInstall_ResponseHasRegId()
        {
            // Arrange
            var unitUnderTest = this.CreateAppStartupController();
            ColdRequestModel coldRequestModel = new ColdRequestModel
            {
                DeviceInfo = new DeviceRegisterModel
                {
                    Handle = "some handle",
                    Platform = PlatFormId.WindowsPushNotificationService
                }
            };
            var expected = "RegistrationId";
            mockHubRegistrationService
                .Setup(hrs => hrs.CreateHubRegistration(It.IsAny<string>()))
                .ReturnsAsync(new HubRegistrationModel { RegistrationId = expected });
            _auto0ConfigProvider
                .SetupGet(acp => acp.CurrentValue)
                .Returns(new Auth0Config());
            
            // Act
            var result = ((await unitUnderTest.Post(coldRequestModel)) as ObjectResult)?.Value as StartupResponseModel;

            // Assert
            Assert.AreEqual(expected, result?.HubRegistration.RegistrationId);
        }

        [TestMethod]
        public async Task Post_Register_ResponseHasRegId()
        {
            // Arrange
            var unitUnderTest = this.CreateAppStartupController();
            unitUnderTest.SetUserClaims(new []{new Claim(ClaimTypes.Email, "some email"),});
            var requestModel = new RegisterUserRequestModel
            {
                DeviceInfo = new DeviceRegisterModel
                {
                    Handle = "some handle",
                    Platform = PlatFormId.WindowsPushNotificationService
                },
                UserInfo = new UserRegisterModel
                {
                    UserName = "some username"
                },
                Registration = new HubRegistrationModel
                {
                    RegistrationId = "some reg id"
                }
            };
            var expected = "RegistrationId";
            mockHubRegistrationService
                .Setup(hrs => hrs.UpdateHubRegistration(It.IsAny<string>(), It.IsAny<PlatFormId>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new UpdateRegistrationResponseModel { RegistrationId = expected });
            mockUserAuthenticationService
                .Setup(uas => uas.Add3rdPartyUser(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new AddResult {User = new IdentityUser()});
            _auto0ConfigProvider
                .SetupGet(acp => acp.CurrentValue)
                .Returns(new Auth0Config());
            
            // Act
            var result = ((await unitUnderTest.PostUser(requestModel)) as ObjectResult)?.Value as StartupResponseModel;

            // Assert
            Assert.AreEqual(expected, result?.HubRegistration.RegistrationId);
        }

        [TestMethod]
        public async Task Post_Register_CreatesUser()
        {
            // Arrange
            var unitUnderTest = this.CreateAppStartupController();
            string expectedEmail = "some email";
            unitUnderTest.SetUserClaims(new []{new Claim(ClaimTypes.Email, expectedEmail),});
            var expected = "some username";
            var requestModel = new RegisterUserRequestModel
            {
                DeviceInfo = new DeviceRegisterModel
                {
                    Handle = "some handle",
                    Platform = PlatFormId.WindowsPushNotificationService
                },
                UserInfo = new UserRegisterModel
                {
                    UserName = expected
                },
                Registration = new HubRegistrationModel
                {
                    RegistrationId = "some reg id"
                }
            };
            mockHubRegistrationService
                .Setup(hrs => hrs.UpdateHubRegistration(It.IsAny<string>(), It.IsAny<PlatFormId>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new UpdateRegistrationResponseModel { RegistrationId = "RegistrationId" });
            mockUserAuthenticationService
                .Setup(uas => uas.Add3rdPartyUser(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new AddResult {User = new IdentityUser()});
            _auto0ConfigProvider
                .SetupGet(acp => acp.CurrentValue)
                .Returns(new Auth0Config());
            
            // Act
            var result = ((await unitUnderTest.PostUser(requestModel)) as ObjectResult)?.Value as StartupResponseModel;

            // Assert
            
            mockUserAuthenticationService
                .Verify(uas => uas.Add3rdPartyUser(expected, expectedEmail));
        }

        
    }

    public static class ControllerExtensions
    {
        public static void SetUserClaims(this ControllerBase controller, IEnumerable<Claim> claims)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new TestPrincipal(claims.ToArray())
                }
            };
        }
    }
}
