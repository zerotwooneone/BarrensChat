using Autofac;
using ChatUw.Settings;

namespace ChatUw.Http
{
    public class AutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterInstance(SettingsCache.GetInstance())
                .As<IAuthenticationCache>()
                .SingleInstance();
            builder
                .RegisterType<LocalHostTestHttpClientFactory>()
                .As<HttpClientFactory>();
        }
    }
}