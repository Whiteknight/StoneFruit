using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Handlers;

namespace StoneFruit.Tests.Utility;

public class TypeNameVerbExtractorTests
{
    private class FirstTestHandler : IHandlerBase
    {
    }

    [Test]
    public void GetVerbs_CamelCaseToSpinalCase()
    {
        var target = new CamelCaseVerbExtractor();
        var result = target.GetVerbs(typeof(FirstTestHandler)).ToList();
        result[0].ToString().Should().Be("first test");
    }
}
