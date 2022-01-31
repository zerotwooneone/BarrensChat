using System;
using Windows.Foundation.Collections;
using Windows.Storage;
using ChatUw.Authentication;
using ChatUw.NotificationHub;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChatUw.Settings
{
    public class SettingsCache : IAuthenticationCache, 
        IRegistrationCache,
        INotificationChannelCache
    {
        private readonly IPropertySet _settings;
        public static SettingsCache GetInstance() => new SettingsCache(ApplicationData.Current.LocalSettings.Values);

        public SettingsCache(IPropertySet settings)
        {
            _settings = settings;
        }
        public virtual AuthModel GetAuthenticationToken()
        {
            return _settings.Get<AuthModel>("AuthenticationModel");
        }

        public virtual void SetAuthenticationToken(AuthModel authModel)
        {
            _settings.Set("AuthenticationModel", authModel);
        }

        public RegistrationModel GetRegistration()
        {
            var json = (string)_settings["RegistrationModel"];
            if (string.IsNullOrWhiteSpace(json)) return null;

            var obj = (JObject)JsonConvert.DeserializeObject(json);
            var id = obj.Value<string>("Id");
            var dateTime = obj.Value<DateTime>("Expiration");
            return new RegistrationModel(id, dateTime);
        }

        public void SetRegistration(RegistrationModel registrationModel)
        {
            _settings["RegistrationModel"] = JsonConvert.SerializeObject(registrationModel);
        }

        public NotificationChannelModel GetNotificationChannel()
        {
            return _settings.Get<NotificationChannelModel>("NotificationChannel");
        }

        public void SetNotificationChannel(NotificationChannelModel notificationChannel)
        {
            _settings.Set("NotificationChannel", notificationChannel);
        }
    }

    public static class PropertySetExtensions
    {
        public static T Get<T>(this IPropertySet propertySet, string key)
        {
            var val = (string)propertySet[key];

            if (val == null) return default(T);

            var result = JsonConvert.DeserializeObject<T>(val);
            return result;
        }

        public static void Set<T>(this IPropertySet propertySet, string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            propertySet[key] = json;
        }
    }
}