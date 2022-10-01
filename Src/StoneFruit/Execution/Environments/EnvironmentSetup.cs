using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Environments
{
    /// <summary>
    /// Sets up the environment mechanism
    /// </summary>
    public class EnvironmentSetup : IEnvironmentSetup
    {
        private readonly IServiceCollection _services;
        private static readonly IReadOnlyList<string> _defaultNamesList = new[] { Constants.EnvironmentNameDefault };

        public EnvironmentSetup(IServiceCollection services)
        {
            _services = services;
        }

        public void BuildUp(IServiceCollection services)
        {
            _services.TryAddSingleton(new EnvironmentsList(_defaultNamesList));
            _services.TryAddSingleton<IEnvironmentCollection>(new InstanceEnvironmentCollection());
        }

        public IEnvironmentSetup SetEnvironments(IReadOnlyList<string> names)
        {
            if (names == null || names.Count == 0)
                names = _defaultNamesList;
            _services.AddSingleton(new EnvironmentsList(names));
            _services.AddSingleton<IEnvironmentCollection, FactoryEnvironmentCollection>();
            return this;
        }

        public IEnvironmentSetup UseFactory<T>(IEnvironmentFactory<T> factory)
            where T : class
        {
            Assert.ArgumentNotNull(factory, nameof(factory));
            _services.AddSingleton(factory);
            _services.AddScoped(services =>
            {
                var factory = services.GetRequiredService<IEnvironmentFactory<T>>();
                var environments = services.GetRequiredService<IEnvironmentCollection>();
                var currentEnvName = environments.GetCurrentName();
                if (!currentEnvName.HasValue)
                    throw new EngineBuildException("Attempt to get environment context object without setting a valid environment");
                var obj = factory.Create(currentEnvName.Value);
                if (!obj.HasValue)
                    throw new EngineBuildException("Could not create valid environment context object for current environment");
                return obj.Value;
            });
            return this;
        }

        public IEnvironmentSetup UseInstance<T>(T environment)
             where T : class
        {
            Assert.ArgumentNotNull(environment, nameof(environment));
            return UseFactory<T>(new InstanceEnvironmentFactory<T>(environment));
        }

        //public IEnvironmentSetup UseInstances(IReadOnlyDictionary<string, object> environments)
        //{
        //    Assert.ArgumentNotNull(environments, nameof(environments));
        //    _environments = new FactoryEnvironmentCollection();
        //    return this;
        //}

        public IEnvironmentSetup None()
        {
            _services.AddSingleton<IEnvironmentCollection>(new InstanceEnvironmentCollection());
            return this;
        }
    }

    public sealed class EnvironmentsList
    {
        public EnvironmentsList(IReadOnlyList<string> validNames)
        {
            ValidNames = validNames;
        }

        public IReadOnlyList<string> ValidNames { get; }
    }
}
