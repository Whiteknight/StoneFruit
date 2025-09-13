namespace StoneFruit.Execution.IO;

public interface IColorOutputFactory
{
    IOutput Create(IOutput inner, Brush brush);
}
