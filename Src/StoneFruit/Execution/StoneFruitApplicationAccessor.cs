using System;

namespace StoneFruit.Execution;

// Accessor object to deal with circular references in the DI container before the Engine
// is registered. Not intended for general purpose external use.
internal class StoneFruitApplicationAccessor
{
    private StoneFruitApplication? _instance;

    public StoneFruitApplication Instance
        => _instance
        ?? throw new InvalidOperationException($"Cannot access {nameof(StoneFruitApplication).GetType().Name} because one has not been created");

    public void Set(StoneFruitApplication engine)
    {
        _instance = engine;
    }
}
