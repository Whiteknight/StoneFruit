using System.Collections.Generic;

namespace StoneFruit.Execution.Environments
{
    /// <summary>
    /// An IEnvironmentCollection implementation for a single environment
    /// </summary>
    public class InstanceEnvironmentCollection : IEnvironmentCollection
    {
        public InstanceEnvironmentCollection(object environment)
        {
            Current = environment ?? new object();
            CurrentName = "";
        }

        public IReadOnlyList<string> GetNames() => new [] { Constants.EnvironmentNameDefault };

        public void SetCurrent(string name)
        {
        }

        public string CurrentName { get; }

        public bool IsValid(string name) => name == Constants.EnvironmentNameDefault;

        public object Current { get; }
    }
}