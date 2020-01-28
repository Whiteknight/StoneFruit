using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Tests.Helpers
{
    public class TestTerminalOutput : ITerminalOutput
    {
        private readonly Queue<string> _inputs;

        public TestTerminalOutput(params string[] inputs)
        {
            Lines = new List<string>();
            _inputs = new Queue<string>(inputs);
        }

        public List<string> Lines { get; }

        public ITerminalOutput Color(Brush brush) => this;

        public ITerminalOutput WriteLine()
        {
            Lines.Add("");
            return this;
        }

        public ITerminalOutput WriteLine(string line)
        {
            Lines.Add(line);
            return this;
        }

        public ITerminalOutput Write(string str)
        {
            // This isn't perfect, but it's good enough for our test purposes
            Lines.Add(str);
            return this;
        }

        public string Prompt(string prompt, bool mustProvide = true, bool keepHistory = true)
        {
            return _inputs.Any() ? _inputs.Dequeue() : string.Empty;
        }
    }
}
