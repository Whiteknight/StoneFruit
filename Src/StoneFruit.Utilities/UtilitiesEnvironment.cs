using System.Collections.Generic;

namespace StoneFruit.Utilities
{
    public class UtilitiesEnvironment
    {
        public IReadOnlyList<string> Libraries { get; }

        public UtilitiesEnvironment()
        {
            Libraries = new[]
            {
                "StoneFruit",
                "StoneFruit.Containers.Lamar",
                "StoneFruit.Containers.Microsoft",
                "StoneFruit.Containers.Ninject",
                "StoneFruit.Containers.StructureMap"
            };
        }
    }
}