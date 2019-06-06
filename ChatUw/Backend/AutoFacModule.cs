using Autofac;

namespace ChatUw.Backend
{
    public class AutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpClientBackendClient>()
                .As<IBackendClient>();
        }
    }
}