using System.Threading.Tasks;

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
        Task<string> CreateRegistration(string token);
    }

    public static class RegistrationServiceExtensions
    {
        public static async Task<RegistrationModel> CreateAndSaveRegistration(this IRegistrationService registrationService,
            string token)
        {
            var regId = await registrationService.CreateRegistration(token);
            var reg = registrationService.SetRegistration(regId);
            return reg;
        }
    }
}