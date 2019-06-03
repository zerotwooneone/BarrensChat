namespace ChatUw.NotificationHub
{
    public interface IRegistrationCache
    {
        RegistrationModel GetRegistration();
        void SetRegistration(RegistrationModel registrationModel);
    }

    public static class RegistrationCacheExtensions
    {
        public static bool HasRegistrationId(this IRegistrationCache registrationCache)
        {
            var registrationModel = registrationCache.GetRegistration();
            return registrationModel != null;
        }
    }
}