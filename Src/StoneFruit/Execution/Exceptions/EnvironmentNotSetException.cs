namespace StoneFruit.Execution.Exceptions;

public class EnvironmentNotSetException : InternalException
{
    public EnvironmentNotSetException(string message) : base(message)
    {
    }

    public static EnvironmentNotSetException Create()
        => new EnvironmentNotSetException("Cannot access the current environment because it is not set. Please confirm that you have specified an environment before attempting to access it.");
}
