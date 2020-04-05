namespace StoneFruit
{
    /// <summary>
    /// Setup the output streams
    /// </summary>
    public interface IOutputSetup
    {
        /// <summary>
        /// Do not use a Console output. Console is the only output which supports color,
        /// so removing it makes color operations into no-ops. Without the console,
        /// Interactive mode in the Engine will exit immediately. This is used mostly
        /// for unit test scenarios.
        /// </summary>
        /// <returns></returns>
        IOutputSetup DoNotUseConsole();

        /// <summary>
        /// Add a secondary output to receive a copy of output text. This text will not include color and
        /// will not participate with interactive prompts.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        IOutputSetup Add(IOutput output);
    }
}