namespace ChatUw.Backend
{
    public class RegisterUserResponse
    {
        public ClientAuthConfigModel ClientAuth { get; set; }
        public HubRegistrationModel HubRegistration { get; set; }
        public AuthenticationModel Authentication { get; set; }
        public bool IsSuccess { get; set; }
    }
}