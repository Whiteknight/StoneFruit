using System;

namespace StoneFruit.Execution;

/// <summary>
/// Exception we throw during engine build-up if conflicting or invalid settings have been
/// selected.
/// </summary>
public class EngineBuildException : Exception
{
    public EngineBuildException(string message) : base(message)
    {
    }
}
