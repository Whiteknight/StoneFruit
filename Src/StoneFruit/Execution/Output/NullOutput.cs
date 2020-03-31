using System;

namespace StoneFruit.Execution.Output
{
    public class NullOutput : IOutput
    {
        public IOutput Color(Func<Brush, Brush> brush) => this;

        public IOutput WriteLine() => this;

        public IOutput WriteLine(string line) => this;

        public IOutput Write(string str) => this;

        public string Prompt(string prompt, bool mustProvide = true, bool keepHistory = true) => string.Empty;
    }
}