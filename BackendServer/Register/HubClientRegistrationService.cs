using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BackendServer.HubClient;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace BackendServer.Register
{
    public class HubClientRegistrationService : IHubRegistrationService
    {
        private readonly HubClientFactory _hubClientFactory;

        public HubClientRegistrationService(HubClientFactory hubClientFactory)
        {
            _hubClientFactory = hubClientFactory;
        }
        public async Task<HubRegistrationModel> CreateHubRegistration(string handle)
        {
            string newRegistrationId = null;
            var hub = _hubClientFactory.Primary;
            // make sure there are no existing registrations for this push handle (used for iOS and Android)
            if (handle != null)
            {
                var registrations = await hub.GetRegistrationsByChannelAsync(handle, 100);

                foreach (RegistrationDescription registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                    }
                    else
                    {
                        await hub.DeleteRegistrationAsync(registration);
                    }
                }
            }

            if (newRegistrationId == null)
            {
                newRegistrationId = await hub.CreateRegistrationIdAsync();
            }

            return new HubRegistrationModel
            {
                RegistrationId = newRegistrationId
            };
        }

        public async Task<UpdateRegistrationResponseModel> UpdateHubRegistration(string handle, PlatFormId platform, string registrationId, string userName)
        {
            RegistrationDescription registration;
            switch (platform)
            {
                case PlatFormId.MicrosoftPushNotificationService:
                    registration = new MpnsRegistrationDescription(handle);
                    break;
                case PlatFormId.WindowsPushNotificationService:
                    registration = new WindowsRegistrationDescription(handle);
                    break;
                case PlatFormId.ApplePushNotificationService:
                    registration = new AppleRegistrationDescription(handle);
                    break;
                case PlatFormId.FirebaseCloudMessaging:
                    registration = new FcmRegistrationDescription(handle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform));
            }

            registration.RegistrationId = registrationId;
            
            // add check if user is allowed to add these tags
            registration.Tags = new HashSet<string>();
            registration.Tags.Add("username:" + userName);

            try
            {
                var hub = _hubClientFactory.Primary;
                var registrationDescription = await hub.CreateOrUpdateRegistrationAsync(registration);
                return new UpdateRegistrationResponseModel
                {
                    RegistrationId = registrationDescription.RegistrationId
                };
            }
            catch (MessagingException e)
            {
                var webex = e.InnerException as WebException;
                if (webex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = (HttpWebResponse)webex.Response;
                    if (response.StatusCode == HttpStatusCode.Gone)
                    {
                        return null;
                    }
                }

                throw;
            }
        }

        public async Task DeleteHubRegistration(string id)
        {
            var hub = _hubClientFactory.Primary;
            await hub.DeleteRegistrationAsync(id);
        }
    }
}