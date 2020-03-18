using System;
using System.Collections.Generic;

namespace StoneFruit.Execution.Environments
{
    public class EnvironmentSetup : IEnvironmentSetup
    {
        private IEnvironmentCollection _environments;

        public IEnvironmentCollection Build() => _environments ?? new InstanceEnvironmentCollection(null);

        /// <summary>
        /// Specify a factory for available environments, if the user should be able to
        /// select from multiple options
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IEnvironmentSetup UseFactory(IEnvironmentFactory factory)
        {
            EnsureEnvironmentsNotSet();
            _environments = factory == null ? null : new FactoryEnvironmentCollection(factory);
            return this;
        }

        /// <summary>
        /// Specify a single environment to use. An environment may represent configuration
        /// or execution-context information
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Specify that the application does not use an environment
        /// </summary>
        /// <returns></returns>
        public IEnvironmentSetup None() => UseInstance(null);

        private void EnsureEnvironmentsNotSet()
        {
            if (_environments != null)
                throw new Exception("Environments are already configured for this builder. You cannot set environments again");
        }
    }
}
