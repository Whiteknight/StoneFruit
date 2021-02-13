using System.Collections.Generic;

namespace StoneFruit.Execution.Environments
{
    /// <summary>
    /// An IEnvironmentCollection implementation for a single environment
    /// </summary>
    public class InstanceEnvironmentCollection : IEnvironmentCollection
    {
        public InstanceEnvironmentCollection()
        {
            Current = new object();
            CurrentName = "";
        }

        public InstanceEnvironmentCollection(object environment)
        {
            Current = environment ?? new object();
            CurrentName = "";
        }

        public IReadOnlyList<string> GetNames() => new[] { Constants.EnvironmentNameDefault };

        public void SetCurrent(string name)
        {
            // This collection represents a singleton instance, there's nothing to set
        }

        public string CurrentName { get; }

        public bool IsValid(string name) => name == Constants.EnvironmentNameDefault;

        public object Current { get; }
    }
}
