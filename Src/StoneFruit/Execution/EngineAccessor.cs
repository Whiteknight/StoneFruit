using System;

namespace StoneFruit.Execution
{
    // Accessor object to deal with circular references in the DI container before the Engine
    // is registered
    public class EngineAccessor
    {
        private Engine? _engine;

        public Engine Engine => _engine ?? throw new InvalidOperationException("Cannot access engine because one has not been created");

        public void SetEngine(Engine engine)
        {
            _engine = engine;
        }
    }
}
