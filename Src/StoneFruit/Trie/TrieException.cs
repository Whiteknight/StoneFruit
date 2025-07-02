using System;

namespace StoneFruit.Trie;

/// <summary>
/// Exception that is thrown during trie operations.
/// </summary>
[Serializable]
public class TrieException : System.Exception
{
    public TrieException(string message)
        : base(message)
    {
    }

    public static TrieException NoArguments()
        => new TrieException("Could not look up an appropriate handler, no verbs were provided");
}
