using System;

namespace StoneFruit.Execution;

// Accessor object to deal with circular references in the DI container before the Engine
// is registered. Not intended for general purpose external use.
internal class EngineAccessor
{
    private StoneFruitApplication? _engine;

    public StoneFruitApplication Engine
        => _engine
        ?? throw new InvalidOperationException("Cannot access engine because one has not been created");

    public void SetEngine(StoneFruitApplication engine)
    {
        _engine = engine;
    }
}
