using System.Collections.Generic;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Segregated interface to get candidate verbs from an IArguments object. This is separated
    /// out from IArguments because we don't want the end-user to be calling these in a handler
    /// </summary>
    public interface IVerbSource
    {
        IReadOnlyList<IPositionalArgument> GetVerbCandidatePositionals();
        void SetVerbCount(int count);
    }
}