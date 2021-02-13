using System;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.CommandSources;
using StoneFruit.Execution.Handlers;
using StoneFruit.Handlers;
using StoneFruit.Utility;

namespace StoneFruit.Execution
{
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
