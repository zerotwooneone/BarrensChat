using System;
using Windows.Foundation.Collections;
using Windows.Storage;
using ChatUw.Http;
using ChatUw.NotificationHub;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    }
}