using System;
using ChatUw.Authentication;
using ChatUw.Backend;
using ChatUw.Http;
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

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockMessageViewmodelFactory = this.mockRepository.Create<IMessageViewmodelFactory>();
            this._registrationService = this.mockRepository.Create<IRegistrationService>();
            _authenticationService = mockRepository.Create<IAuthenticationService>();
            _backendClient = mockRepository.Create<IBackendClient>();
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
                _backendClient.Object);
        }

        [TestMethod]
        public void LoginVisility_BrandNewInstallation_LoginCanExecute()
        {
            // Arrange
            _authenticationService
                .Setup(ats=>ats.GetValidAuthModel())
                .Returns((AuthModel)null);
            _registrationService
                .Setup(rc => rc.GetValidRegistrationFromCache())
                .Returns((RegistrationModel) null);
            var unitUnderTest = this.CreateMainPageViewmodel();

            // Act
            var actual = unitUnderTest.LoginCommand.CanExecute(null);

            // Assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void LoadedCommand_BrandNewInstallation_SetsToken()
        {
            var unitUnderTest = this.CreateMainPageViewmodel();
            _authenticationService
                .Setup(ats=>ats.GetValidAuthModel())
                .Returns((AuthModel)null);
            _registrationService
                .Setup(rs => rs.GetValidRegistrationFromCache())
                .Returns((RegistrationModel) null);
            const string expected = "some token";
            _authenticationService
                .Setup(ats => ats.Get3rdPartyAuth())
                .ReturnsAsync(new LoginModel {Token = expected});
            _authenticationService
                .Setup(ats => ats.SetAuthModel(It.IsAny<string>()))
                .Verifiable();

            // Act
            unitUnderTest.LoadedCommand.Execute(null);

            // Assert
            _authenticationService
                .Verify(ats => ats.SetAuthModel(It.Is<string>(am=>am == expected)), Times.Once);
        }

        [TestMethod]
        public void LoadedCommand_RecentlyLoggedIn_SendEnabled()
        {
            var unitUnderTest = this.CreateMainPageViewmodel();
            _authenticationService
                .Setup(ats=>ats.GetValidAuthModel())
                .Returns(new AuthModel{Token = "some token"});
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
                .Returns(new AuthModel{Token = expected});
            _registrationService
                .Setup(rs => rs.GetValidRegistrationFromCache())
                .Returns((RegistrationModel)null);
            _registrationService
                .Setup(rs => rs.CreateRegistration(It.IsAny<string>()))
                .ReturnsAsync("some reg");
            

            // Act
            unitUnderTest.LoadedCommand.Execute(null);

            // Assert
            _registrationService
                .Verify(rs => rs.CreateRegistration(It.Is<string>(s=>s==expected)), Times.Once);
        }

        [TestMethod]
        public void LoadedCommand_AuthExpired_UpdatesRegistration()
        {
            var unitUnderTest = this.CreateMainPageViewmodel();
            const string expected = "some token";
            _authenticationService
                .Setup(ats=>ats.GetValidAuthModel())
                .Returns((AuthModel)null);
            _registrationService
                .Setup(rs => rs.GetValidRegistrationFromCache())
                .Returns(new RegistrationModel("id", DateTime.Parse("2019/06/05")));
            _authenticationService
                .Setup(rs => rs.Get3rdPartyAuth())
                .ReturnsAsync(new LoginModel{Token = expected});
            _authenticationService
                .Setup(ats=>ats.SetAuthModel(It.IsAny<string>()))
                .Returns(new AuthModel{Token = "some token"});
            
            // Act
            unitUnderTest.LoadedCommand.Execute(null);

            // Assert
            _registrationService
                .Verify(rs => rs.CreateRegistration(It.Is<string>(s=>s==expected)), Times.Once);
        }
    }
}
