using System.Collections.Generic;
using StoneFruit.BuiltInVerbs;

namespace StoneFruit.Execution
{
    public class EngineEventCatalog
    {
        public EngineEventCatalog()
        {
            // Events when the engine starts and then stops headless mode
            EngineStartHeadless = new EventScript();
            EngineStopHeadless = new EventScript();

            // Events when the engine starts and stops interactive mode
            EngineStartInteractive = new EventScript(
                $"{EnvironmentChangeVerb.NotSetName}"
            );
            EngineStopInteractive = new EventScript();
            
            // Event when the environment has been successfully changed
            EnvironmentChanged = new EventScript();

            // Attempt to enter headless mode without providing any arguments
            HeadlessNoArgs = new EventScript(
                $"{EchoVerb.Name} 'Please provide a verb'",
                // We exit here so the engine doesn't try to continue execution
                $"{ExitVerb.Name} {Engine.ExitCodeOk}"
            );

            // TODO: It would be nice to be able to pass the name of the unknown verb here, so we could
            // make suggestions or give more insight
            // Attempt to execute an unknown verb
            VerbNotFound = new EventScript(
                $"{EchoVerb.Name} 'Verb not found. Please check your spelling or help output and try again.'"
            );
        }

        public class EventScript
        {
            private readonly string[] _initialLines;
            private readonly List<string> _lines;

            public EventScript(params string[] initialLines)
            {
                _initialLines = initialLines;
                _lines = new List<string>(initialLines);
            }

            public void Reset()
            {
                _lines.Clear();
                _lines.AddRange(_initialLines);
            }

            public void Clear() => _lines.Clear();

            public void Add(params string[] lines) => _lines.AddRange(lines);

            public void EnqueueScript(EngineState state)
            {
                foreach (var line in _lines)
                    state.AddCommand(line);
            }

            public override string ToString() => string.Join("\n", _lines);
        }

        public EventScript HeadlessNoArgs { get; }
        public EventScript EngineStartHeadless { get; }
        public EventScript EngineStartInteractive { get; }
        public EventScript EngineStopInteractive { get; }
        public EventScript EngineStopHeadless { get; }
        public EventScript VerbNotFound { get; }
        public EventScript EnvironmentChanged { get; }
    }
}