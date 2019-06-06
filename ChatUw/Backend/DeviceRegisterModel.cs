namespace ChatUw.Backend
{
    public class DeviceRegisterModel
    {
        public int Platform => 2; //PlatFormId.MicrosoftPushNotificationService - WNS
        public string Handle { get; set; }
    }
}