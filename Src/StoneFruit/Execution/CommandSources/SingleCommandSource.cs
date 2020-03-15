namespace StoneFruit.Execution.CommandSources
{
    // TODO: Change this so it contains a finite queue of commands instead of just one.
    public class SingleCommandSource : ICommandSource
    {
        private readonly string _command;
        private string _queued;

        public SingleCommandSource(string command)
        {
            _command = command;
            _queued = null;
        }

        public void Start()
        {
            _queued = _command;
        }

        public string GetNextCommand()
        {
            var value = _queued;
            _queued = null;
            return value;
        }
    }
}