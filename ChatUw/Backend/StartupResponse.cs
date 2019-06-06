namespace ChatUw.Backend
{
    public class StartupResponse
    {
        public ClientAuthConfigModel ClientAuth { get; set; }
        public HubRegistrationModel HubRegistration { get; set; }
        public bool IsSuccess { get; set; }
    }
}