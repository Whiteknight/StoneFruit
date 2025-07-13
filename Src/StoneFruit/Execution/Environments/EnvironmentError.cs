using StoneFruit.Execution.Exceptions;

namespace StoneFruit.Execution.Environments;

public abstract record EnvironmentError();
public sealed record NoEnvironmentSpecified() : EnvironmentError;
public sealed record NoEnvironmentSpecifiedHeadless() : EnvironmentError;
public sealed record InvalidEnvironment(string NameOrNumber) : EnvironmentError;
public sealed record NoEnvironmentSet() : EnvironmentError;
public sealed class EnvironmentsException : InternalException
{
    public EnvironmentsException(string message) : base(message)
    {
    }

    public static EnvironmentError Throw(EnvironmentError err)
    {
        return err switch
        {
            NoEnvironmentSpecified => throw new EnvironmentsException("No environment specified and no default could be selected."),
            NoEnvironmentSpecifiedHeadless => throw new EnvironmentsException("No environment specified in headless mode and no default could be selected."),
            InvalidEnvironment ie => throw new EnvironmentsException($"Invalid environment {ie.NameOrNumber} specified. Environment not changed."),
            NoEnvironmentSet => err,
            _ => err
        };
    }
}