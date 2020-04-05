using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Environments;

namespace StoneFruit.Tests.Execution.Environments
{
    public class DictionaryEnvironmentFactoryTests
    {
        [Test]
        public void Create_ValidName()
        {
            var instance = new object();
            var target = new DictionaryEnvironmentFactory(new Dictionary<string, object> { { "test", instance } });
            target.Create("test").Should().BeSameAs(instance);
        }

        [Test]
        public void Create_InvalidName()
        {
            var instance = new object();
            var target = new DictionaryEnvironmentFactory(new Dictionary<string, object> { { "test", instance } });
            target.Create("GARBAGE").Should().BeNull();
        }

        [Test]
        public void ValidEnvironments_Test()
        {
            var instance = new object();
            var target = new DictionaryEnvironmentFactory(new Dictionary<string, object> { 
                { "a", instance },
                { "b", instance },
                { "c", instance }
            });
            var result = target.ValidEnvironments.ToList();
            result.Should().Contain(new[] { "a", "b", "c" });
        }
    }
}
