using System;

namespace StoneFruit.Execution
{
    public class ColoredTerminalOutputWrapper : ITerminalOutput
    {
        private readonly Brush _color;
        private readonly ITerminalOutput _inner;

        public ColoredTerminalOutputWrapper(Brush color, ITerminalOutput inner)
        {
            _color = color;
            _inner = inner;
        }

        public ITerminalOutput Color(Brush brush) => new ColoredTerminalOutputWrapper(brush, _inner);

        public ITerminalOutput WriteLine()
        {
            WithBrush(() => _inner.WriteLine());
            return this;
        }

        public ITerminalOutput WriteLine(string line)
        {
            WithBrush(() => _inner.WriteLine(line));
            return this;
        }

        public ITerminalOutput Write(string str)
        {
            WithBrush(() => _inner.Write(str));
            return this;
        }

        public string Prompt(string prompt, bool mustProvide = true, bool keepHistory = true) 
            => _inner.Prompt(prompt, mustProvide, keepHistory);

        private void WithBrush(Action act)
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
        }
    }
}