namespace ChatUw.Backend
{
    public class StartupResponseModel
    {
        public ClientAuthConfigModel ClientAuth { get; set; }
        public HubRegistrationModel HubRegistration { get; set; }
        public AuthenticationModel Authentication { get; set; }
    }
}