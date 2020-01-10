using System.Collections.Generic;

namespace StoneFruit
{
    /// <summary>
    /// Provides a list of valid environments and creates environments by name
    /// </summary>
    public interface IEnvironmentFactory
    {
        object Create(string name);
        IReadOnlyCollection<string> ValidEnvironments { get; }
    }
}