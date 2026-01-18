namespace StoneFruit.Execution.IO;

public sealed record StringCommandLine(string CommandLine) : ICommandLine
{
    public string GetRawArguments() => CommandLine;
}
