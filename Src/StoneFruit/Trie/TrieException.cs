// Disable the warning about exception constructors. We don't want them here because we're not
// using them and they count against us in terms of code lines not covered by unit tests.
#pragma warning disable RCS1194

namespace StoneFruit.Trie
{
    /// <summary>
    /// Exception that is thrown during trie operations
    /// </summary>
    [System.Serializable]
    public class TrieException : System.Exception
    {
        public TrieException(string message) : base(message)
        {
        }

        protected TrieException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public static TrieException NoArguments()
            => new TrieException("Could not look up an appropriate handler, no verbs were provided");
    }
}

#pragma warning restore RCS1194 // Implement exception constructors.
