using BackendServer.Register;

namespace BackendServer.AppStartup
{
    public class DeviceRegisterModel
    {
        public PlatFormId Platform { get; set; }
        public string Handle { get; set; }
    }
}