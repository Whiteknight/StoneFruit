namespace StoneFruit
{
    public interface IEnvironmentSetup
    {
        IEnvironmentSetup UseEnvironmentFactory(IEnvironmentFactory factory);
        IEnvironmentSetup UseEnvironment(object environment);
        IEnvironmentSetup NoEnvironment();
    }
}