using System.Collections.Generic;
using StoneFruit.Execution.Templating;

namespace StoneFruit.Execution.IO;

public class NullIO : IOutput, IInput, ITemplateParser, ITemplate
{
    public IEnumerable<OutputMessage> Render(object? value)
    {
        return [];
    }

    public Maybe<string> Prompt(string prompt, bool mustProvide = true, bool keepHistory = true)
        => default;

    public ITemplate Parse(string format) => this;

    public IOutput WriteMessage(OutputMessage message) => this;
}
