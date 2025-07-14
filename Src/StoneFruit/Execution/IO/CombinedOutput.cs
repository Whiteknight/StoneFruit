using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.IO;

public class CombinedOutput : IOutput
{
    private readonly IOutput _primary;
    private readonly IReadOnlyList<IOutput> _secondaries;

    public CombinedOutput(IEnumerable<IOutput> secondaries)
    {
        _primary = new NullIO();
        _secondaries = secondaries.OrEmptyIfNull().ToList();
    }

    public CombinedOutput(IOutput primary, IEnumerable<IOutput> secondaries)
    {
        _primary = primary;
        _secondaries = secondaries.OrEmptyIfNull().ToList();
    }

    public IOutput Color(Func<Brush, Brush> changeBrush)
    {
        if (_primary == null || changeBrush == null)
            return this;
        var newPrimary = _primary.Color(changeBrush);
        return ReferenceEquals(newPrimary, _primary)
            ? this
            : new CombinedOutput(newPrimary, _secondaries);
    }

    public IOutput WriteLine()
    {
        _primary?.WriteLine();
        foreach (var s in _secondaries)
            s.WriteLine();
        return this;
    }

    public IOutput WriteLine(string line)
    {
        _primary?.WriteLine(line);
        foreach (var s in _secondaries)
            s.WriteLine(line);
        return this;
    }

    public IOutput Write(string str)
    {
        _primary?.Write(str);
        foreach (var s in _secondaries)
            s.Write(str);
        return this;
    }
}
