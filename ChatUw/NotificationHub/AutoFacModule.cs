using Autofac;
using ChatUw.Settings;

namespace ChatUw.NotificationHub
{
    public class AutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterInstance(SettingsCache.GetInstance())
                .As<IRegistrationCache>()
                .SingleInstance();
            builder
                .RegisterType<RegistrationService>()
                .As<IRegistrationService>()
                .SingleInstance();
            builder
                .RegisterType<RegisterClient>()
                .SingleInstance();
            builder
                .RegisterType<PushNotificationChannelProvider>()
                .SingleInstance();
        }
    }
}