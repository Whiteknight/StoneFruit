using System.Collections.Generic;

namespace StoneFruit
{
    public interface IEnvironmentFactory
    {
        object Create(string name);
        IReadOnlyCollection<string> ValidEnvironments { get; }
    }
}