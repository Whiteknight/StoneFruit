namespace StoneFruit.Execution
{
    /// <summary>
    /// Empty CommandCounter implementation for situations where counting is not necessary.
    /// </summary>
    public class NullCommandCounter : IEngineStateCommandCounter
    {
        public void ReceiveUserInput()
        {
        }

        public bool VerifyCanExecuteNextCommand(ICommandParser parser, IOutput output) => true;
    }
}
