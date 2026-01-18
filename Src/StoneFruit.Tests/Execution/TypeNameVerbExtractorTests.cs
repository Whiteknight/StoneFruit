using AwesomeAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Handlers;

namespace StoneFruit.Tests.Utility;

public class TypeNameVerbExtractorTests
{
    private class FirstTestHandler : IHandler
    {
        public void Execute(IArguments arguments, HandlerContext context)
            => throw new System.NotImplementedException();
    }

    [Test]
    public void GetVerbs_CamelCaseToSpinalCase()
    {
        var target = new CamelCaseVerbExtractor();
        var result = target.GetVerbs(typeof(FirstTestHandler)).GetValueOrThrow();
        result[0].ToString().Should().Be("first test");
    }
}
