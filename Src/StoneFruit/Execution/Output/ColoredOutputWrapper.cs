using System;

namespace StoneFruit.Execution.Output
{
    public class ColoredOutputWrapper : IOutput
    {
        private readonly Brush _color;
        private readonly IOutput _inner;

        public ColoredOutputWrapper(Brush color, IOutput inner)
        {
            _color = color;
            _inner = inner;
        }

        public IOutput Color(Brush brush) 
            => brush.Equals(_color) ? this : new ColoredOutputWrapper(brush, _inner);

        public IOutput WriteLine() => WithBrush(() => _inner.WriteLine());

        public IOutput WriteLine(string line) => WithBrush(() => _inner.WriteLine(line));

        public IOutput Write(string str) => WithBrush(() => _inner.Write(str));

        public string Prompt(string prompt, bool mustProvide = true, bool keepHistory = true) 
            => _inner.Prompt(prompt, mustProvide, keepHistory);

        private IOutput WithBrush(Action act)
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
}