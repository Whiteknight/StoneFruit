using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using StoneFruit.Execution.Handlers;

namespace StoneFruit.Tests.Utility
{
    public class TypeNameVerbExtractorTests
    {
        private class FirstTestHandler : IHandlerBase
        {
        }

        [Test]
        public void GetVerbs_CamelCaseToSpinalCase()
        {
            var target = new CamelToSpinalNameVerbExtractor();
            var result = target.GetVerbs(typeof(FirstTestHandler)).ToList();
            result.Should().Contain("first-test");
        }
    }
}
