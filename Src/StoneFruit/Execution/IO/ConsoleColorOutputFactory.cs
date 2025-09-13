namespace StoneFruit.Execution.IO;

public class ConsoleColorOutputFactory : IColorOutputFactory
{
    public IOutput Create(IOutput inner, Brush brush)
        => new ConsoleColorOutput(brush, inner);
}
