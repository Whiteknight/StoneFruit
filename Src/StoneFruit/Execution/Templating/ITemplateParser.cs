namespace StoneFruit.Execution.Templating;

public interface ITemplateParser
{
    ITemplate Parse(string format);
}
