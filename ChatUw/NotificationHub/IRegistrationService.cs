namespace ChatUw.NotificationHub
{
    public interface IRegistrationService
    {
        /// <summary>
        /// Returns a registration from the cache if it is still valid, null otherwise
        /// </summary>
        /// <returns></returns>
        RegistrationModel GetValidRegistrationFromCache();

        RegistrationModel SetRegistration(string id);
    }
}