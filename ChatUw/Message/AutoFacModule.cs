using Autofac;

namespace ChatUw.Message
{
    public class AutoFacModule: Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<ViewmodelFactory>()
                .As<IMessageViewmodelFactory>()
                .SingleInstance();
        }
    }
}
