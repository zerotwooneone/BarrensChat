using System;
using ChatUw.Authentication;
using ChatUw.Backend;
using ChatUw.Message;
using ChatUw.NotificationHub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ChatUw.Tests
{
    [TestClass]
    public class MainPageViewmodelTests
    {
        private MockRepository mockRepository;

        private Mock<IMessageViewmodelFactory> mockMessageViewmodelFactory;
        private Mock<IRegistrationService> _registrationService;
        private Mock<IAuthenticationService> _authenticationService;
        private Mock<IBackendClient> _backendClient;
        //private Mock<INotificationChannelCache> _notificationChannelCache;
        private Mock<PushNotificationChannelProvider> _pushNotificationChannelProvider;
        private Mock<IPushNotificationChannel> _pushNotificationChannel;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockMessageViewmodelFactory = this.mockRepository.Create<IMessageViewmodelFactory>();
            this._registrationService = this.mockRepository.Create<IRegistrationService>();
            _authenticationService = mockRepository.Create<IAuthenticationService>();
            _backendClient = mockRepository.Create<IBackendClient>();
            //_notificationChannelCache = mockRepository.Create<INotificationChannelCache>();
            _pushNotificationChannelProvider = mockRepository.Create<PushNotificationChannelProvider>();
            _pushNotificationChannel = mockRepository.Create<IPushNotificationChannel>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.mockRepository.VerifyAll();
        }

        private MainPageViewmodel CreateMainPageViewmodel()
        {
            return new MainPageViewmodel(
                this.mockMessageViewmodelFactory.Object,
                this._registrationService.Object,
                _authenticationService.Object,
                _backendClient.Object,
                //_notificationChannelCache.Object,
                _pushNotificationChannelProvider.Object);
        }

        [TestMethod]
        public void LoadedCommand_BrandNewInstallation_SendsHandle()
        {
            var unitUnderTest = this.CreateMainPageViewmodel();
            _authenticationService
                .Setup(ats=>ats.GetValidAuthModel())
                .Returns((AuthModel)null);
            _registrationService
                .Setup(rs => rs.GetValidRegistrationFromCache())
                .Returns((RegistrationModel) null);
            _authenticationService
                .Setup(ats => ats.Get3rdPartyAuth())
                .ReturnsAsync(new LoginModel {Token = "some token", Username = "some username"});
            _authenticationService
                .Setup(ats => ats.SetAuthModel(It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();
            //_notificationChannelCache
            //    .Setup(ncc => ncc.GetNotificationChannel())
            //    .Returns((NotificationChannelModel) null);
            //_notificationChannelCache
            //    .Setup(ncc => ncc.SetNotificationChannel(It.IsAny<NotificationChannelModel>()))
            //    .Verifiable();
            _pushNotificationChannelProvider
                .Setup(pcp => pcp.CreateNotificationChannel())
                .ReturnsAsync(_pushNotificationChannel.Object);
            const string expected = "some handle";
            _pushNotificationChannel
                .SetupGet(pnc => pnc.Uri)
                .Returns(expected);
            _backendClient
                .Setup(bec => bec.AppStartup(It.IsAny<string>()))
                .ReturnsAsync(new StartupResponse
                {
                    ClientAuth = new ClientAuthConfigModel(),
                    HubRegistration = new HubRegistrationModel(),
                    IsSuccess = true
                });

            // Act
            unitUnderTest.LoadedCommand.Execute(null);

            // Assert
            _backendClient
                .Verify(bec => bec.AppStartup(It.Is<string>(s=>s==expected)), Times.Once);
        }

        [TestMethod]
        public void LoadedCommand_RecentlyLoggedIn_SendEnabled()
        {
            var unitUnderTest = this.CreateMainPageViewmodel();
            _authenticationService
                .Setup(ats=>ats.GetValidAuthModel())
                .Returns(new AuthModel{Token = "some token", Username = "some username"});
            _registrationService
                .Setup(rs => rs.GetValidRegistrationFromCache())
                .Returns(new RegistrationModel("id", DateTime.Parse("2019/06/05")));
            

            // Act
            unitUnderTest.LoadedCommand.Execute(null);

            // Assert
            Assert.IsTrue(unitUnderTest.SendCommand.CanExecute(null));
        }

        [TestMethod]
        public void LoadedCommand_RegExpired_RefreshesReg()
        {
            var unitUnderTest = this.CreateMainPageViewmodel();
            const string expected = "some token";
            _authenticationService
                .Setup(ats=>ats.GetValidAuthModel())
                .Returns(new AuthModel{Token = expected, Username = "some username"});
            _registrationService
                .Setup(rs => rs.GetValidRegistrationFromCache())
                .Returns((RegistrationModel)null);
            _pushNotificationChannelProvider
                .Setup(pcp => pcp.CreateNotificationChannel())
                .ReturnsAsync(_pushNotificationChannel.Object);
            _pushNotificationChannel
                .SetupGet(pnc => pnc.Uri)
                .Returns("some handle");
            _backendClient
                .Setup(rs => rs.RegisterUser(It.IsAny<string>(), 
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new RegisterUserResponse
                {
                    Authentication = new AuthenticationModel(),
                    ClientAuth = new ClientAuthConfigModel(),
                    HubRegistration = new HubRegistrationModel()
                });
            

            // Act
            unitUnderTest.LoadedCommand.Execute(null);

            // Assert
            _backendClient
                .Verify(rs => rs.RegisterUser(It.IsAny<string>(), 
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.Is<string>(actual=>actual==expected)), 
                    Times.Once);
        }
    }
}
