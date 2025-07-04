using System;

namespace StoneFruit.Execution.Output;

public class NullOutput : IOutput
{
    public IOutput Color(Func<Brush, Brush> changeBrush) => this;

    public IOutput WriteLine() => this;

    public IOutput WriteLine(string line) => this;

    public IOutput Write(string str) => this;

    public Maybe<string> Prompt(string prompt, bool mustProvide = true, bool keepHistory = true)
        => default;
}