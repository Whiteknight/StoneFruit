using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Utility;

namespace StoneFruit.Tests.Utility
{
    public class TypeExtensionsTests
    {
        private class FirstTestHandler : IHandlerBase
        {
        }

        [Test]
        public void GetVerbs_CamelCaseToSpinalCase()
        {
            var result = typeof(FirstTestHandler).GetVerbs().ToList();
            result.Should().Contain("first-test");
        }
    }
}
