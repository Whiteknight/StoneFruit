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

        public IReadOnlyDictionary<int, string> GetNames() => new Dictionary<int, string> { { 0, "" } };

        public string GetName(int index) => "";

        public void SetCurrent(string name)
        {
        }

        public void SetCurrent(int index)
        {
        }

        public string CurrentName { get; }

        public bool IsValid(string name) => name == "";

        public bool IsValid(int index) => index == 1;

        public object Get(string name) => Current;

        public object Get(int idx) => Current;

        public object Current { get; }
    }
}