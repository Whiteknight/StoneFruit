namespace StoneFruit.Utility
{
    [System.Serializable]
    public class TrieException : System.Exception
    {
        public TrieException() { }
        public TrieException(string message) : base(message) { }
        public TrieException(string message, System.Exception inner) : base(message, inner) { }
        protected TrieException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public static TrieException NoArguments()
        {
            return new TrieException("Could not look up an appropriate handler, no verbs were provided");
        }
    }
}
