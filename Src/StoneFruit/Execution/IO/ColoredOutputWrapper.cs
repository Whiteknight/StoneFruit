using System;

namespace StoneFruit.Execution.IO;

public class ColoredOutputWrapper : IOutput
{
    private readonly Brush _color;
    private readonly IOutput _inner;

    public ColoredOutputWrapper(Brush color, IOutput inner)
    {
        _color = color;
        _inner = inner;
    }

    public IOutput Color(Func<Brush, Brush> changeBrush)
    {
        var newBrush = changeBrush?.Invoke(_color);
        return !newBrush.HasValue || newBrush == _color
            ? this
            : new ColoredOutputWrapper(newBrush.Value, _inner);
    }

    public IOutput WriteLine() => WithBrush(() => _inner.WriteLine());

    public IOutput WriteLine(string line) => WithBrush(() => _inner.WriteLine(line));

    public IOutput Write(string str) => WithBrush(() => _inner.Write(str));

    private ColoredOutputWrapper WithBrush(Action act)
    {
        var current = Brush.Current;
        _color.Set();
        try
        {
            act();
        }
        finally
        {
            current.Set();
        }

        return this;
    }
}