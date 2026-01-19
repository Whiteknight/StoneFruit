using AwesomeAssertions;
using NUnit.Framework;
using StoneFruit.Execution.IO;
using StoneFruit.Execution.Templating;

namespace StoneFruit.Tests.Execution.IO;

public class NullIoTests
{
    [Test]
    public void Parse_ReturnsSameInstance()
    {
        var nullIo = new NullIO();
        var parsed = ((ITemplateParser)nullIo).Parse("anything");
        parsed.Should().BeSameAs(nullIo);
    }

    [Test]
    public void Render_AlwaysReturnsEmptyCollection()
    {
        var nullIo = new NullIO();

        var emptyForNull = ((ITemplate)nullIo).Render(null);
        var emptyForObject = ((ITemplate)nullIo).Render(new { Prop = "value" });

        emptyForNull.Should().BeEmpty();
        emptyForObject.Should().BeEmpty();
    }

    [Test]
    public void Prompt_ReturnsEmptyMaybe()
    {
        var nullIo = new NullIO();
        var maybe = ((IInput)nullIo).Prompt("prompt");
        maybe.IsSuccess.Should().BeFalse();
    }

    [Test]
    public void WriteMessage_ReturnsSameInstance_And_DefaultWriteLine_Chains()
    {
        var nullIo = new NullIO();

        var returned = nullIo.WriteMessage(new OutputMessage("x"));
        returned.Should().BeSameAs(nullIo);

        IOutput asOutput = nullIo;
        var lineReturned = asOutput.WriteLine("hello");
        lineReturned.Should().BeSameAs(nullIo);
    }
}
