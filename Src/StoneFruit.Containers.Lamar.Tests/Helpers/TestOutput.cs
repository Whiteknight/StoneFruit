using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Containers.Lamar.Tests.Helpers
{
    public class TestOutput : IOutput
    {
        private readonly Queue<string> _inputs;

        public TestOutput(params string[] inputs)
        {
            Lines = new List<string>();
            _inputs = new Queue<string>(inputs);
        }

        public List<string> Lines { get; }

        public IOutput Color(Brush brush) => this;

        public IOutput WriteLine()
        {
            Lines.Add("");
            return this;
        }

        public IOutput WriteLine(string line)
        {
            Lines.Add(line);
            return this;
        }

        public IOutput Write(string str)
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
