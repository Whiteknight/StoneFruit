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

        public IResult<ArgumentsOrString> GetNextCommand()
        {
            var currentNameResult = _environments.GetCurrentName();
            var currentName = currentNameResult.HasValue ? currentNameResult.Value : "";

            var str = _output.Prompt($"{currentName}");
            _state.CommandCounter.ReceiveUserInput();
            return string.IsNullOrEmpty(str) ? FailureResult<ArgumentsOrString>.Instance : Result.Success(new ArgumentsOrString(str));
        }
    }
}
