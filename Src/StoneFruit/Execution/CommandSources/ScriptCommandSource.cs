using System.Linq;
using StoneFruit.Execution.Scripts;

namespace StoneFruit.Execution.CommandSources;

/// <summary>
/// An adaptor for EventScript to ICommandSource. Makes commands from a script
/// available to the runloop.
/// </summary>
public class ScriptCommandSource : ICommandSource
{
    private readonly ArgumentsOrString[] _script;
    private int _index;

    public ScriptCommandSource(EventScript script, ICommandParser parser, IArguments args)
    {
        _script = script.GetCommands(parser, args).ToArray();
        _index = 0;
    }

    public Maybe<ArgumentsOrString> GetNextCommand()
        => _index >= _script.Length
            ? default(Maybe<ArgumentsOrString>)
            : _script[_index++];
}
