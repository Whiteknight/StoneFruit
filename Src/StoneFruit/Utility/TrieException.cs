namespace StoneFruit.Utility
{
    /// <summary>
    /// Exception that is thrown during trie operations
    /// </summary>
    [System.Serializable]
    public class TrieException : System.Exception
    {
        public TrieException(string message) : base(message) { }
        protected TrieException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public static TrieException NoArguments()
        {
            return new TrieException("Could not look up an appropriate handler, no verbs were provided");
        }
    }
}
