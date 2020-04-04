using System;
using System.Collections.Generic;

namespace StoneFruit.Execution.Environments
{
    /// <summary>
    /// Sets up the environment mechanism
    /// </summary>
    public class EnvironmentSetup : IEnvironmentSetup
    {
        private IEnvironmentCollection _environments;

        public IEnvironmentCollection Build() => _environments ?? new InstanceEnvironmentCollection(null);

        public IEnvironmentSetup UseFactory(IEnvironmentFactory factory)
        {
            if (factory == null)
            {
                _environments = null;
                return this;
            }

            EnsureEnvironmentsNotSet();
            _environments = new FactoryEnvironmentCollection(factory);
            return this;
        }

        public IEnvironmentSetup UseInstance(object environment)
        {
            EnsureEnvironmentsNotSet();
            _environments = new InstanceEnvironmentCollection(environment);
            return this;
        }

        public IEnvironmentSetup UseInstances(IReadOnlyDictionary<string, object> environments)
        {
            if (environments == null)
            {
                _environments = null;
                return this;
            }

            EnsureEnvironmentsNotSet();
            _environments = new FactoryEnvironmentCollection(new DictionaryEnvironmentFactory(environments));
            return this;
        }

        public IEnvironmentSetup None() => UseInstance(null);

        private void EnsureEnvironmentsNotSet()
        {
            if (_environments != null)
                throw new EngineBuildException("Environments are already configured for this builder. You cannot set environments again");
        }
    }
}
