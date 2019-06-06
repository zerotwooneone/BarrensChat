using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ChatUw.Providers;

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
                .RegisterAssemblyModules(typeof(MainPage).Assembly);
        }
    }
}
