using System.Collections.Generic;

namespace StoneFruit.Execution.Environments
{
    public class InstanceEnvironmentCollection : IEnvironmentCollection
    {
        public InstanceEnvironmentCollection(object environment)
        {
            Current = environment ?? new object();
            CurrentName = "";
        }

        public IReadOnlyDictionary<int, string> GetNames()
        {
            return new Dictionary<int, string>
            {
                { 0, "" }
            };
        }

        public string GetName(int index)
        {
            return "";
        }

        public void SetCurrent(string name)
        {
        }

        public void SetCurrent(int index)
        {
        }

        public string CurrentName { get; }

        public bool IsValid(string name)
        {
            return name == "";
        }

        public bool IsValid(int index)
        {
            return index == 1;
        }

        public object Get(string name)
        {
            return Current;
        }

        public object Get(int idx)
        {
            return Current;
        }

        public object Current { get; }
    }
}