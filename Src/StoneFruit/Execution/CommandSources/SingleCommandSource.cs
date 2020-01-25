namespace StoneFruit.Execution.CommandSources
{
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