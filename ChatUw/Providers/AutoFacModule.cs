using Autofac;

namespace ChatUw.Providers
{
    public class AutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterInstance(new CurrentDateTimeProvider())
                .SingleInstance();
        }
    }
}