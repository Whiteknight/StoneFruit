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

        public string AsString(string _ = null) => bool.TrueString;

        public bool AsBool(bool _ = false) => true;

        public int AsInt(int _ = 0) => 1;

        public long AsLong(long _ = 0) => 1L;
    }
}
