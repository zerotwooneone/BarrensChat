using System;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using ChatUw.Providers;

namespace ChatUw.NotificationHub
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationCache _registrationCache;
        private readonly CurrentDateTimeProvider _currentDateTimeProvider;
        private readonly RegisterClient _registerClient;
        private readonly PushNotificationChannelProvider _pushNotificationChannelProvider;

        public RegistrationService(IRegistrationCache registrationCache,
            CurrentDateTimeProvider currentDateTimeProvider,
            RegisterClient registerClient,
            PushNotificationChannelProvider pushNotificationChannelProvider)
        {
            _registrationCache = registrationCache;
            _currentDateTimeProvider = currentDateTimeProvider;
            _registerClient = registerClient;
            _pushNotificationChannelProvider = pushNotificationChannelProvider;
        }

        public RegistrationModel GetValidRegistrationFromCache()
        {
            var reg = _registrationCache.GetRegistration();
            return reg == null
                ? null
                : reg.Expiration < _currentDateTimeProvider.GetCurrentDateTime()
                    ? null
                    : reg;
        }

        public RegistrationModel SetRegistration(string id)
        {
            var expiration = DateTime.Now.Add(MagicValues.RegistrationLifeTime);
            var registrationModel = new RegistrationModel(id, expiration);
            _registrationCache.SetRegistration(registrationModel);
            return registrationModel;
        }

        public async Task<string> CreateRegistration(string token)
        {
            var channel = await _pushNotificationChannelProvider.CreateNotificationChannel();

            channel.PushNotificationReceived += OnPushNotificationReceived;

            var registrationModel = GetValidRegistrationFromCache();
            string regId;
            if (registrationModel ==null)
            {
                regId = await _registerClient.RequestNewRegistrationAsync(token);
            }
            else
            {
                if (await _registerClient.TryUpdateRegistrationAsync(registrationModel.Id, channel.Uri, token))
                {
                    regId = registrationModel.Id;
                }
                else
                {
                    regId = await _registerClient.RequestNewRegistrationAsync(token);
                }
            }

            return regId;
        }

        private void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            int x = 0;
        }
    }
}