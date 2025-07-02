namespace StoneFruit.Execution;

/// <summary>
/// Source for commands to be executed. Command sources are forward-only and are expected to
/// only be modified by the user, not internally by the Engine.
/// </summary>
public interface ICommandSource
{
    /// <summary>
    /// Get the next command from the source. If there are no more commands, returns null
    /// </summary>
    /// <returns></returns>
    IResult<ArgumentsOrString> GetNextCommand();
}
