namespace StoneFruit
{
    public interface IVerbInfo
    {
        string Verb { get; }

        string Description { get; }
        string Help { get; }
        bool ShouldShowInHelp { get; }
    }
}