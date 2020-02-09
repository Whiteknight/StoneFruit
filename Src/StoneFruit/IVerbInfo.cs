namespace StoneFruit
{
    public interface IVerbInfo
    {
        string Verb { get; }

        string Description { get; }
        string Usage { get; }
        bool ShouldShowInHelp { get; }
    }
}