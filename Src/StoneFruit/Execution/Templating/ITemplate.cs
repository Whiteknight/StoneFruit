namespace StoneFruit.Execution.Templating;

public interface ITemplate
{
    void Render(IOutput output, object? value);
}
