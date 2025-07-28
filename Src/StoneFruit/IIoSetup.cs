using StoneFruit.Execution.Templating;

namespace StoneFruit;

/// <summary>
/// Setup the output streams.
/// </summary>
public interface IIoSetup
{
    IIoSetup UseOutput<T>()
        where T : class, IOutput;

    IIoSetup UseOutput(IOutput output);

    IIoSetup UseInput<T>()
        where T : class, IInput;

    IIoSetup UseInput(IInput input);

    IIoSetup SetCommandLine(ICommandLine commandLine);

    IIoSetup SetCommandLine(string commandLine);

    IIoSetup UseTemplateParser(ITemplateParser parser);
}
