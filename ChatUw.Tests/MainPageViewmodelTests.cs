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
        private Mock<IAuthenticationCache> mockAuthenticationCache;
        private Mock<IRegistrationService> _registrationService;
        private Mock<HttpClientFactory> mockHttpClientFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockMessageViewmodelFactory = this.mockRepository.Create<IMessageViewmodelFactory>();
            this.mockAuthenticationCache = this.mockRepository.Create<IAuthenticationCache>();
            this._registrationService = this.mockRepository.Create<IRegistrationService>();
            this.mockHttpClientFactory = this.mockRepository.Create<HttpClientFactory>();
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
                this.mockAuthenticationCache.Object,
                this._registrationService.Object,
                this.mockHttpClientFactory.Object);
        }

        [TestMethod]
        public void LoginVisility_BrandNewInstallation_LoginCanExecute()
        {
            // Arrange
            mockAuthenticationCache
                .Setup(ac => ac.GetAuthenticationToken())
                .Returns((string)null);
            _registrationService
                .Setup(rc => rc.GetValidRegistrationFromCache())
                .Returns((RegistrationModel) null);
            var unitUnderTest = this.CreateMainPageViewmodel();

            // Act
            var actual = unitUnderTest.LoginCommand.CanExecute(null);

            // Assert
            Assert.AreEqual(true, actual);
        }
    }
}
