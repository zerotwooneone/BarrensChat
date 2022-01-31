using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ChatUw.Container
{
    public class Builder
    {
        public IContainer Initialize()
        {
            var builder = new ContainerBuilder();

            SetupServices(builder);

            return builder.Build();
        }

        private void SetupServices(ContainerBuilder builder)
        {
            builder
                .RegisterType<MainPageViewmodel>();
            builder
                .RegisterInstance(new MemoryCache(new OptionsManager<MemoryCacheOptions>(new OptionsFactory<MemoryCacheOptions>(new List<IConfigureOptions<MemoryCacheOptions>>(), new List<IPostConfigureOptions<MemoryCacheOptions>>()))))
                .As<IMemoryCache>()
                .SingleInstance();
            
            builder
                .RegisterAssemblyModules(typeof(MainPage).Assembly);
        }
    }
}
