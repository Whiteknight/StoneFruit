namespace StoneFruit.Execution.Arguments;

/// <summary>
/// A raw argument value from the parser. The full understanding of what type of
/// argument this is might not be fully determined until an attempt is made to
/// access it. This interface is just a flag, the IArguments objects will work with specific
/// concrete types of each argument.
/// </summary>
public interface IParsedArgument
{
}
