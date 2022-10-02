using System;
using StoneFruit.Execution;

namespace StoneFruit.Handlers
{
    [Verb(Name, Hide = true)]
    public class MetadataHandler : IHandler
    {
        public const string Name = "metadata";
        public const string RemoveSubverb = "remove";
        public const string ListSubverb = "list";

        private readonly EngineState _state;
        private readonly IArguments _args;
        private readonly IOutput _output;

        public MetadataHandler(EngineState state, IArguments args, IOutput output)
        {
            _state = state;
            _args = args;
            _output = output;
        }

        public static string Group => HelpHandler.BuiltinsGroup;
        public static string Description => "Work with internal metadata";

        public static string Usage => @"metadata ...

metadata remove <keys>...
    Remove a list of metadata items by name

metadata list
    List all available metadata entries
";

        public void Execute()
        {
            var subverb = _args.Shift().Require("Must provide an action").AsString();
            if (subverb == ListSubverb)
            {
                foreach (var kvp in _state.Metadata)
                {
                    _output.Color(ConsoleColor.Green).Write(kvp.Key);
                    _output.Write(": ");
                    _output.WriteLine(kvp.Value.GetType().Name);
                }

                return;
            }

            if (subverb == RemoveSubverb)
            {
                foreach (var name in _args.GetAllPositionals())
                    _state.Metadata.Remove(name.AsString());
                return;
            }

            throw new ExecutionException("Unknown arguments");
        }
    }
}
