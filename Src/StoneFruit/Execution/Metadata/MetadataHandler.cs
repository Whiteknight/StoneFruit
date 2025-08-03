using System;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Help;

namespace StoneFruit.Execution.Metadata;

[Verb(Name, Hide = true)]
public class MetadataHandler : IHandler
{
    public const string Name = "_metadata";
    public const string RemoveSubverb = "remove";
    public const string ListSubverb = "list";

    public static string Group => HelpHandler.BuiltinsGroup;
    public static string Description => "Work with internal metadata";

    public static string Usage => $"""
        {Name} ...

        {Name} remove <keys>...
            Remove a list of metadata items by name

        {Name} list
            List all available metadata entries
        """;

    public void Execute(IArguments arguments, HandlerContext context)
    {
        var output = context.Output;
        var state = context.State;
        var subverb = arguments.Shift().Require("Must provide an action").AsString();
        if (subverb == ListSubverb)
        {
            foreach (var kvp in state.Metadata)
            {
                output.Color(ConsoleColor.Green).Write(kvp.Key);
                output.Write(": ");
                output.WriteLine(kvp.Value.ToString() ?? string.Empty);
            }

            return;
        }

        if (subverb == RemoveSubverb)
        {
            foreach (var name in arguments.GetAllPositionals())
                state.Metadata.Remove(name.AsString());
            return;
        }

        throw new ExecutionException("Unknown arguments");
    }
}
