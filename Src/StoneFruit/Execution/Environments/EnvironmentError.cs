using StoneFruit.Execution.Exceptions;

namespace StoneFruit.Execution.Environments;

public abstract record EnvironmentError(string Message);

// Tried to set or interact with an environment, but no name/number is provided
public sealed record NoEnvironmentSpecified() : EnvironmentError("No environment name or number provided");

// Same as above, but more serious because it's headless mode and we can't recover
public sealed record NoEnvironmentSpecifiedHeadless() : EnvironmentError("No environment name or number provided in headless mode");

// A name of an environment was specified, but the name is invalid
public sealed record InvalidEnvironment(string NameOrNumber) : EnvironmentError("Invalid environment");

// There is no current environment to interact with
public sealed record NoEnvironmentSet() : EnvironmentError("No environment set");

// An environment change attempt was made but no change occured. Maybe the same one?
public sealed record EnvironmentNotChanged() : EnvironmentError("Environment was not successfully changed");

public sealed class EnvironmentsException : InternalException
{
    public EnvironmentsException(string message) : base(message)
    {
    }

    public static EnvironmentError Throw(EnvironmentError err)
        => err switch
        {
            NoEnvironmentSpecified => throw new EnvironmentsException("No environment specified and no default could be selected."),
            NoEnvironmentSpecifiedHeadless => throw new EnvironmentsException("No environment specified in headless mode and no default could be selected."),
            InvalidEnvironment ie => throw new EnvironmentsException($"Invalid environment {ie.NameOrNumber} specified. Environment not changed."),
            NoEnvironmentSet => err,
            EnvironmentNotChanged => err,
            _ => err
        };
}
