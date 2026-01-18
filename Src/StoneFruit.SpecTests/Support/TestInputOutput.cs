using System.Text;
using StoneFruit.Execution.Templating;

namespace StoneFruit.SpecTests.Support;

public class TestInputOutput : IOutput, IInput
{
    private readonly Queue<string> _inputs;
    private readonly StringBuilder _output;
    private readonly ITemplateParser _templateParser;

    public TestInputOutput(ITemplateParser parser, params string[] inputs)
    {
        _output = new StringBuilder();
        _inputs = new Queue<string>(inputs);
        _templateParser = parser;
    }

    public string[] Lines => _output.ToString()
        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
        .Select(l => l.Trim())
        .ToArray();

    public void EnqueueInputLine(string line)
        => _inputs.Enqueue(line);

    public Maybe<string> Prompt(string prompt, bool mustProvide = true, bool keepHistory = true)
    {
        return _inputs.Any() ? new Maybe<string>(_inputs.Dequeue()) : default;
    }

    public IOutput WriteMessage(OutputMessage message)
    {
        if (message.IsTemplate)
        {
            var template = _templateParser.Parse(message.Text ?? string.Empty);
            var messages = template.Render(message.Object.GetValueOrDefault(new object()));
            foreach (var msg in messages)
                WriteMessage(msg with { IsError = message.IsError });
            if (message.IncludeNewline)
                _output.AppendLine();
            return this;
        }

        _output.Append(message.Text);
        if (message.IncludeNewline)
            _output.AppendLine();

        return this;
    }
}
