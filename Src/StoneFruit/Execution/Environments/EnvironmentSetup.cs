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
            EnsureEnvironmentsNotSet();
            _environments = factory == null ? null : new FactoryEnvironmentCollection(factory);
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
            EnsureEnvironmentsNotSet();
            _environments = environments == null ? null : new FactoryEnvironmentCollection(new DictionaryEnvironmentFactory(environments));
            return this;
        }

        public IEnvironmentSetup None() => UseInstance(null);

        private void EnsureEnvironmentsNotSet()
        {
            if (_environments != null)
                throw new Exception("Environments are already configured for this builder. You cannot set environments again");
        }
    }
}
