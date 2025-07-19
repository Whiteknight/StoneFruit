using System.Collections.Generic;

namespace StoneFruit.Execution.Arguments;

public sealed class Arguments : IArguments, IVerbSource
{
    private readonly IArgumentCollection _args;

    public Arguments(IArgumentCollection args, ArgumentValueMapper mapper)
    {
        _args = args;
        Mapper = mapper;
    }

    public static Arguments Empty { get; } = new Arguments(SyntheticArguments.Empty, ArgumentValueMapper.Empty);

    public string Raw => _args.Raw;

    public ArgumentValueMapper Mapper { get; }

    public IPositionalArgument Get(int index)
        => _args.Get(index);

    public INamedArgument Get(string name, bool caseSensitive = true)
        => _args.Get(name, caseSensitive);

    public IEnumerable<IFlagArgument> GetAllFlags()
        => _args.GetAllFlags();

    public IEnumerable<INamedArgument> GetAllNamed(string name, bool caseSensitive = true)
        => _args.GetAllNamed(name, caseSensitive);

    public IEnumerable<INamedArgument> GetAllNamed()
        => _args.GetAllNamed();

    public IEnumerable<IPositionalArgument> GetAllPositionals()
        => _args.GetAllPositionals();

    public IFlagArgument GetFlag(string name, bool caseSensitive = true)
        => _args.GetFlag(name, caseSensitive);

    public IReadOnlyList<string> GetUnconsumed() => _args.GetUnconsumed();
    public IReadOnlyList<IPositionalArgument> GetVerbCandidatePositionals()
        => (_args as IVerbSource)?.GetVerbCandidatePositionals() ?? [];

    public bool HasFlag(string name, bool markConsumed = false, bool caseSensitive = true)
        => _args.HasFlag(name, markConsumed, caseSensitive);

    public void Reset() => _args.Reset();
    public void SetVerbCount(int count) => (_args as IVerbSource)?.SetVerbCount(count);
    public IPositionalArgument Shift() => _args.Shift();
}
