using ChatUw.NotificationHub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using ChatUw.Http;
using ChatUw.Providers;

namespace ChatUw.Tests.NotificationHub
{
    [TestClass]
    public class RegistrationServiceTests
    {
        private MockRepository mockRepository;

        private Mock<IRegistrationCache> mockRegistrationCache;
        private Mock<CurrentDateTimeProvider> _currentDateTimeProvider;
        private Mock<PushNotificationChannelProvider> _pushNotificationChannelProvider;
        private Mock<RegisterClient> _registerClient;
        private Mock<HttpClientFactory> _httpClientFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockRegistrationCache = this.mockRepository.Create<IRegistrationCache>();
            _currentDateTimeProvider = mockRepository.Create<CurrentDateTimeProvider>();
            _httpClientFactory = mockRepository.Create<HttpClientFactory>();
            _registerClient = mockRepository.Create<RegisterClient>(_httpClientFactory.Object);
            _pushNotificationChannelProvider = mockRepository.Create<PushNotificationChannelProvider>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.mockRepository.VerifyAll();
        }

        private RegistrationService CreateService()
        {
            return new RegistrationService(
                this.mockRegistrationCache.Object,
                _currentDateTimeProvider.Object,
                _registerClient.Object,
                _pushNotificationChannelProvider.Object);
        }

        [TestMethod]
        public void GetValidRegistrationFromCache_ExpiredReg_IsNull()
        {
            // Arrange
            var unitUnderTest = this.CreateService();
            var now = DateTime.Parse("2019/06/01");
            var expired = now.AddDays(-1);
            mockRegistrationCache
                .Setup(rc => rc.GetRegistration())
                .Returns(new RegistrationModel("id",expired));
            _currentDateTimeProvider
                .Setup(dtp => dtp.GetCurrentDateTime())
                .Returns(now);

            // Act
            var result = unitUnderTest.GetValidRegistrationFromCache();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SetRegistration_WithRegId_ExpectedId()
        {
            // Arrange
            var unitUnderTest = this.CreateService();
            string expected = "expected";
            mockRegistrationCache
                .Setup(rc=>rc.SetRegistration(It.IsAny<RegistrationModel>()))
                .Verifiable();

            // Act
            unitUnderTest.SetRegistration(
                expected);

            // Assert
            mockRegistrationCache
                .Verify(rc=>rc.SetRegistration(It.Is<RegistrationModel>(rm=>rm.Id == expected)));
        }
    }
}
