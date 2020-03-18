using System.Collections.Generic;

namespace StoneFruit
{
    public interface IEnvironmentSetup
    {
        IEnvironmentSetup UseFactory(IEnvironmentFactory factory);
        IEnvironmentSetup UseInstance(object environment);
        IEnvironmentSetup UseInstances(IReadOnlyDictionary<string, object> environments);
        IEnvironmentSetup None();
    }
}