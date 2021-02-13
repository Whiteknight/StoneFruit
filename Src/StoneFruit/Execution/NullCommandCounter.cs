using System;
using System.Threading;
using StoneFruit.Utility;

namespace StoneFruit.Execution
{

    public class NullCommandCounter : IEngineStateCommandCounter
    {
        public void ReceiveUserInput()
        {
        }

        public bool VerifyCanExecuteNextCommand(ICommandParser parser, IOutput output) => true;
    }
}
