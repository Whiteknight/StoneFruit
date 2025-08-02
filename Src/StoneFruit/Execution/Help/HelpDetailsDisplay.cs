using StoneFruit.Execution.Exceptions;

namespace StoneFruit.Execution.Help;

public class HelpDetailsDisplay
{
    private readonly IOutput _output;
    private readonly IHandlers _commands;

    public HelpDetailsDisplay(IOutput output, IHandlers commands)
    {
        _output = output;
        _commands = commands;
    }

    public void ShowTestDetail(Verb verb)
    {
        _commands.GetByName(verb)
            .OnSuccess(v => _output.WriteLine(v.Usage))
            .OnFailure(() => throw new ExecutionException($"Cannot find command named '{verb}'"));
    }
}