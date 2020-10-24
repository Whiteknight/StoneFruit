using System.Collections.Generic;

namespace StoneFruit.Execution.Arguments
{
    public interface IVerbSource
    {
        IReadOnlyList<IPositionalArgument> GetVerbCandidatePositionals();
        void SetVerbCount(int count);
    }
}