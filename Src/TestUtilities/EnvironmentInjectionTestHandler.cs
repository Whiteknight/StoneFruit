using StoneFruit;

namespace TestUtilities
{
    [Verb("test-environment")]
    public class EnvironmentInjectionTestHandler : IHandler
    {
        private readonly IOutput _output;
        public TestEnvironment Environment { get; }

        public EnvironmentInjectionTestHandler(TestEnvironment environment, IOutput output)
        {
            _output = output;
            Environment = environment;
        }

        public void Execute()
        {
            _output.WriteLine(Environment.Value ?? "");
        }
    }
}