using StoneFruit.Execution.Templating;

namespace StoneFruit.Execution.IO;

public class NullIO : IOutput, IInput, ITemplateParser, ITemplate
{
    public ITemplateParser TemplateParser => this;

    public IOutput WriteLine(string? line = "", Brush brush = default) => this;

    public IOutput Write(string str, Brush brush = default) => this;

    public Maybe<string> Prompt(string prompt, bool mustProvide = true, bool keepHistory = true)
        => default;

    public ITemplate Parse(string format) => this;

    public void Render(IOutput output, object? value)
    {
    }
}
