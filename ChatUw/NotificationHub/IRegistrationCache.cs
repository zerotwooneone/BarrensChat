namespace ChatUw.NotificationHub
{
    public interface IRegistrationCache
    {
        string GetRegistrationId();
        void SetRegistrationId(string id);
    }

    public static class RegistrationCacheExtensions
    {
        public static bool HasRegistrationId(this IRegistrationCache registrationCache)
        {
            return !string.IsNullOrWhiteSpace(registrationCache.GetRegistrationId());
        }
    }
}