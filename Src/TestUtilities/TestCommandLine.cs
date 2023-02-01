using StoneFruit;

namespace TestUtilities;

public class TestCommandLine : ICommandLine
{
    private readonly string _commandLine;

    public TestCommandLine(string commandLine)
    {
        _commandLine = commandLine;
    }

    public string GetRawArguments() => _commandLine;
}
