using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Environments
{
    /// <summary>
    /// Sets up the environment mechanism
    /// </summary>
    public class EnvironmentSetup : IEnvironmentSetup, ISetupBuildable<IEnvironmentCollection>
    {
        private IEnvironmentCollection _environments;

        public EnvironmentSetup()
        {
            _environments = new InstanceEnvironmentCollection();
        }

        public void BuildUp(IServiceCollection services)
        {
            IEnvironmentCollection environments = Build();
            services.TryAddSingleton(environments);
        }

        public IEnvironmentCollection Build() => _environments;

        public IEnvironmentSetup UseFactory(IEnvironmentFactory factory)
        {
            Assert.ArgumentNotNull(factory, nameof(factory));
            _environments = new FactoryEnvironmentCollection(factory);
            return this;
        }

        public IEnvironmentSetup UseInstance(object environment)
        {
            Assert.ArgumentNotNull(environment, nameof(environment));
            _environments = new InstanceEnvironmentCollection(environment);
            return this;
        }

        public IEnvironmentSetup UseInstances(IReadOnlyDictionary<string, object> environments)
        {
            Assert.ArgumentNotNull(environments, nameof(environments));
            _environments = new FactoryEnvironmentCollection(new DictionaryEnvironmentFactory(environments));
            return this;
        }

        public IEnvironmentSetup None()
        {
            _environments = new InstanceEnvironmentCollection();
            return this;
        }
    }
}
