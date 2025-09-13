using System.Text;
using StoneFruit.Execution.IO;
using StoneFruit.Execution.Templating;

namespace StoneFruit.SpecTests.Support;

public class TestInputOutput : IOutput, IInput, IColorOutputFactory
{
    private readonly Queue<string> _inputs;
    private readonly StringBuilder _output;

    public TestInputOutput(ITemplateParser parser, params string[] inputs)
    {
        _output = new StringBuilder();
        _inputs = new Queue<string>(inputs);
        TemplateParser = parser;
    }

    public string[] Lines => _output.ToString()
        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
        .Select(l => l.Trim())
        .ToArray();

    public ITemplateParser TemplateParser { get; }

    public IOutput Inner => this;

    public IColorOutputFactory ColorOutputFactory => this;

    public void EnqueueInputLine(string line)
        => _inputs.Enqueue(line);

    public IOutput Color(Func<Brush, Brush> changeBrush) => this;

    public IOutput WriteLine()
    {
        _output.AppendLine();
        return this;
    }

    public IOutput WriteLine(string line)
    {
        _output.AppendLine(line);
        return this;
    }

    public IOutput Write(string str)
    {
        _output.Append(str);
        return this;
    }

    public Maybe<string> Prompt(string prompt, bool mustProvide = true, bool keepHistory = true)
    {
        return _inputs.Any() ? new Maybe<string>(_inputs.Dequeue()) : default;
    }

    public IOutput Create(IOutput inner, Brush brush) => this;
}
