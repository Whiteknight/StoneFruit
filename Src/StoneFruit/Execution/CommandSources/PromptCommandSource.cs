namespace StoneFruit.Execution.CommandSources
{
    /// <summary>
    /// prompts the user to input commands and exposes them to the runloop
    /// </summary>
    public class PromptCommandSource : ICommandSource
    {
        private readonly IOutput _output;
        private readonly IEnvironmentCollection _environments;
        private readonly EngineState _state;

        public PromptCommandSource(IOutput output, IEnvironmentCollection environments, EngineState state)
        {
            _output = output;
            _environments = environments;
            _state = state;
        }

        public ArgumentsOrString GetNextCommand()
        {
            var str = _output.Prompt($"{_environments.CurrentName}");
            _state.CommandCounter.ReceiveUserInput();
            return str;
        }
    }
}