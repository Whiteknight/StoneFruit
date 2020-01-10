using System.Collections.Generic;

namespace StoneFruit
{
    /// <summary>
    /// Manages the list of possible environments. Environments are cached after creation, each environment
    /// is only created once. Environments are ordered and can be accessed by name or number
    /// </summary>
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