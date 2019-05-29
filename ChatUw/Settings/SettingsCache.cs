using Windows.Foundation.Collections;
using Windows.Storage;
using ChatUw.Http;
using ChatUw.NotificationHub;

namespace ChatUw.Settings
{
    public class SettingsCache : IAuthenticationCache, IRegistrationCache
    {
        private readonly IPropertySet _settings;
        public static SettingsCache GetInstance() => new SettingsCache(ApplicationData.Current.LocalSettings.Values);

        public SettingsCache(IPropertySet settings)
        {
            _settings = settings;
        }
        public virtual string GetAuthenticationToken()
        {
            return (string) _settings["AuthenticationToken"];
        }

        public virtual void SetAuthenticationToken(string token)
        {
            _settings["AuthenticationToken"] = token;
        }

        public string GetRegistrationId()
        {
            return (string) _settings["__NHRegistrationId"];
        }

        public void SetRegistrationId(string id)
        {
            _settings["__NHRegistrationId"] = id;
        }
    }
}