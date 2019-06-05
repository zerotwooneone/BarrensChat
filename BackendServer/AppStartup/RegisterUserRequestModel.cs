using BackendServer.Register;

namespace BackendServer.AppStartup
{
    public class RegisterUserRequestModel
    {
        public DeviceRegisterModel DeviceInfo { get; set; }
        public UserRegisterModel UserInfo { get; set; }
        public HubRegistrationModel Registration { get; set; }
    }
}