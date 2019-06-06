using Autofac;
using ChatUw.Settings;

namespace ChatUw.Authentication
{
    public class AutoFacModule : Module{
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JwtAuthService>()
                .As<IAuthenticationService>();
            builder
                .RegisterInstance(SettingsCache.GetInstance())
                .As<IAuthenticationCache>()
                .SingleInstance();
        }
    }
}