using System.Threading.Tasks;

namespace BackendServer.Register
{
    public interface IHubRegistrationService
    {
        Task<HubRegistrationModel> CreateHubRegistration(string handle);
        Task<UpdateRegistrationResponseModel> UpdateHubRegistration(string handle,
            PlatFormId platform,
            string registrationId,
            string userName);
        Task DeleteHubRegistration(string id);
    }

    public static class HubRegistrationServiceExtensions{
        public static Task<HubRegistrationModel> CreateHubRegistration(
            this IHubRegistrationService hubRegistrationService,
            RegistrationRequestModel registrationRequestModel)
        {
            return hubRegistrationService.CreateHubRegistration(registrationRequestModel.Handle);
        }

        public static Task<UpdateRegistrationResponseModel> UpdateHubRegistration(
            this IHubRegistrationService hubRegistrationService,
            UpdateRegistrationRequestModel updateRegistrationRequestModel)
        {
            return hubRegistrationService.UpdateHubRegistration(updateRegistrationRequestModel.Handle,
                updateRegistrationRequestModel.Platform,
                updateRegistrationRequestModel.RegistrationId,
                updateRegistrationRequestModel.Username);
        }
    }
}