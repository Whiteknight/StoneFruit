using AwesomeAssertions;
using NUnit.Framework;

namespace StoneFruit.Tests;

public class VerbTests
{
    [Test]
    public void ctor_string_Null()
    {
        var result = Verb.TryCreate((string)null);
        result.SatisfiesError(e => e is Verb.NoWordsError).Should().BeTrue();
    }

    [Test]
    public void ctor_string_Empty()
    {
        var result = Verb.TryCreate("");
        result.SatisfiesError(e => e is Verb.NoWordsError).Should().BeTrue();
    }

    [Test]
    public void ctor_string_Whitespace()
    {
        var result = Verb.TryCreate("    ");
        result.SatisfiesError(e => e is Verb.NoWordsError).Should().BeTrue();
    }

    [Test]
    public void ctor_stringArray_Null()
    {
        var result = Verb.TryCreate((string[])null);
        result.SatisfiesError(e => e is Verb.NoWordsError).Should().BeTrue();
    }

    [Test]
    public void ctor_stringArray_Empty()
    {
        var result = Verb.TryCreate(new string[0]);
        result.SatisfiesError(e => e is Verb.NoWordsError).Should().BeTrue();
    }

    [Test]
    public void ctor_stringArray_NoValidEntries()
    {
        var result = Verb.TryCreate(new string[] { "", "", null });
        result.SatisfiesError(e => e is Verb.NoWordsError).Should().BeTrue();
    }

    [Test]
    public void SingleWord_String()
    {
        var target = Verb.TryCreate("test");
        target.GetValueOrThrow().Should().BeEquivalentTo(new string[] { "test" });
    }

    [Test]
    public void MultipleWords_String()
    {
        var target = Verb.TryCreate("test foo bar");
        target.GetValueOrThrow().Should().BeEquivalentTo(new string[] { "test", "foo", "bar" });
    }

    [Test]
    public void SingleWord_StringArray()
    {
        var target = Verb.TryCreate(new string[] { "test" });
        target.GetValueOrThrow().Should().BeEquivalentTo(new string[] { "test" });
    }

    [Test]
    public void MultipleWords_StringArray()
    {
        var target = Verb.TryCreate(new string[] { "test", "foo bar" });
        target.GetValueOrThrow().Should().BeEquivalentTo(new string[] { "test", "foo", "bar" });
    }
}
