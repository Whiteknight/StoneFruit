using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Handlers
{
    // TODO: Additional commands to show a list of existing metadata keys and get the .ToString version of
    // a single key
    [Verb(Name, false)]
    public class MetadataRemoveHandler : IHandler
    {
        public const string Name = "metadata-remove";

        private readonly EngineState _state;
        private readonly CommandArguments _args;

        public MetadataRemoveHandler(EngineState state, CommandArguments args)
        {
            _state = state;
            _args = args;
        }

        public void Execute()
        {
            var name = _args.Shift().Require().AsString();
            _state.RemoveMetadata(name);
        }
    }
}