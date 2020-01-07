using System.Collections.Generic;

namespace StoneFruit
{
    public interface IEnvironmentCollection
    {
        IReadOnlyDictionary<int, string> GetNames();
        string GetName(int index);
        void SetCurrent(string name);
        void SetCurrent(int index);
        string CurrentName { get; }
        bool IsValid(string name);
        bool IsValid(int index);
        object Get(string name);
        object Get(int idx);
        object Current { get; }
    }
}