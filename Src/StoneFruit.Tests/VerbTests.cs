using System;
using FluentAssertions;
using NUnit.Framework;

namespace StoneFruit.Tests
{
    public class VerbTests
    {
        [Test]
        public void ctor_string_Null()
        {
            Action act = () => new Verb((string)null);
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void ctor_string_Empty()
        {
            Action act = () => new Verb("");
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void ctor_string_Whitespace()
        {
            Action act = () => new Verb("    ");
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void ctor_stringArray_Null()
        {
            Action act = () => new Verb((string[])null);
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void ctor_stringArray_Empty()
        {
            Action act = () => new Verb(new string[0]);
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void ctor_stringArray_NoValidEntries()
        {
            Action act = () => new Verb(new string[] { "", "", null });
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void SingleWord_String()
        {
            var target = new Verb("test");
            target.Should().BeEquivalentTo(new string[] { "test" });
        }

        [Test]
        public void MultipleWords_String()
        {
            var target = new Verb("test foo bar");
            target.Should().BeEquivalentTo(new string[] { "test", "foo", "bar" });
        }

        [Test]
        public void SingleWord_StringArray()
        {
            var target = new Verb(new string[] { "test" });
            target.Should().BeEquivalentTo(new string[] { "test" });
        }

        [Test]
        public void MultipleWords_StringArray()
        {
            var target = new Verb(new string[] { "test", "foo bar" });
            target.Should().BeEquivalentTo(new string[] { "test", "foo", "bar" });
        }
    }
}
