namespace BackendServer.Register
{
    public class UpdateRegistrationRequestModel
    {
        public string RegistrationId { get; set; }
        public PlatFormId Platform { get; set; }
        public string Handle { get; set; }
        public string Username { get; set; }
    }
}