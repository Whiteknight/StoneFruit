using System;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace StoneFruit.Containers.StructureMap
{
    public static class ContainerExtensions
    {
        public static void SetupEngine<TEnvironment>(this IContainer container, Action<IEngineBuilder> build)
            where TEnvironment : class
        {
            var services = new StructureMapServiceCollection();
            EngineBuilder.SetupEngineRegistrations(services, build);

            services.AddSingleton<IHandlerSource>(provider => new StructureMapHandlerSource(provider, TypeVerbExtractor.DefaultInstance));
            container.Configure(c =>
            {
                c.ScanForCommandVerbs();
                c.For<TEnvironment>().Add(c => c.GetInstance<IEnvironmentCollection>().Current as TEnvironment);
            });
            container.Populate(services);
            //var have = container.WhatDoIHave();
        }
    }
}
