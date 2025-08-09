using System.Collections.Generic;
using System.IO;

namespace StoneFruit;

public static class HandlerSetupScriptFilesExtensions
{
    public static IHandlerSetup AddScript(this IHandlerSetup handlers, Stream stream, Verb verb, string description = "", string usage = "", string group = "")
    {
        using var reader = new StreamReader(stream);
        return handlers.AddScript(reader, verb, description, usage, group);
    }

    public static IHandlerSetup AddScript(this IHandlerSetup handlers, TextReader reader, Verb verb, string description = "", string usage = "", string group = "")
    {
        var lines = ReadAllLinesFromStream(reader);
        return handlers.AddScript(verb, lines, description, usage, group);

        static IEnumerable<string> ReadAllLinesFromStream(TextReader reader)
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }
    }

    public static IHandlerSetup AddScriptFile(this IHandlerSetup handlers, string filePath, Verb verb = default, string description = "", string usage = "", string group = "")
    {
        var lines = File.ReadAllLines(filePath);
        verb = verb.IsValid ? verb : Path.GetFileNameWithoutExtension(filePath);
        return handlers.AddScript(verb, lines, description, usage, group);
    }
}
