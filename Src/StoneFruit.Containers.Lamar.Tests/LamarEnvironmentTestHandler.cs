namespace StoneFruit.Containers.Lamar.Tests
{
    [Verb("test-environment")]
    public class LamarEnvironmentTestHandler : IHandler
    {
        private readonly IOutput _output;
        public LamarTestEnvironment Environment { get; }

        public LamarEnvironmentTestHandler(LamarTestEnvironment environment, IOutput output)
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