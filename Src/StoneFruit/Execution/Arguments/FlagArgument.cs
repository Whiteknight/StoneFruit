namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Accessor for a flag argument which contains a name but no value
    /// </summary>
    public class FlagArgument : IFlagArgument
    {
        public FlagArgument(string name)
        {
            Name = name.ToLowerInvariant();
        }

        public string Name { get; }

        public bool Consumed { get; set; }

        public string AsString(string defaultValue = null) => bool.TrueString;

        public bool AsBool(bool defaultValue = false) => true;

        public int AsInt(int defaultValue = 0) => 1;

        public long AsLong(long defaultValue = 0) => 1L;
    }
}