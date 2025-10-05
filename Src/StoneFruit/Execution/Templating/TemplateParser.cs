namespace StoneFruit.Execution.Templating;

public interface ITemplate
{
    void Render(IOutput output, object? value);
}

public interface ITemplateParser
{
    ITemplate Parse(string format);
}
