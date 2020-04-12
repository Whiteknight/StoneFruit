using System;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace StoneFruit.Containers.StructureMap
{
    public static class ContainerExtensions
    {
        public static void SetupEngine(this IContainer container, Action<IEngineBuilder> build)
        {
            var services = new StructureMapServiceCollection();
            EngineBuilder.SetupEngineRegistrations(services, build);

            services.AddSingleton<IHandlerSource>(provider => new StructureMapHandlerSource(provider, TypeVerbExtractor.DefaultInstance));
            container.Configure(c => c.ScanForCommandVerbs());
            container.Populate(services);
            //var have = container.WhatDoIHave();
        }
    }
}
