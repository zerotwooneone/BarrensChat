﻿using Autofac;

namespace ChatUw.Http
{
    public class AutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<LocalHostTestHttpClientFactory>()
                .As<HttpClientFactory>();
        }
    }
}