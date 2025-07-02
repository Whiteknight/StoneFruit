using System.Collections.Generic;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// Segregated interface to get candidate verbs from an IArguments object. This is separated
/// out from IArguments because we don't want the end-user to be calling these in a handler
/// </summary>
public interface IVerbSource
{
    /// <summary>
    /// Get a list of leading positionals which are candidates for being part of the verb.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<IPositionalArgument> GetVerbCandidatePositionals();

    /// <summary>
    /// Mark a number of leading positionals as being the Verb, the remainder are kept as
    /// positional arguments.
    /// </summary>
    /// <param name="count"></param>
    void SetVerbCount(int count);
}
