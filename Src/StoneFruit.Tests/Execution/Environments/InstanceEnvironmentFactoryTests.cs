using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Environments;

namespace StoneFruit.Tests.Execution.Environments
{
    public class InstanceEnvironmentFactoryTests
    {
        [Test]
        public void ValidEnvironments_Test()
        {
            var target = new InstanceEnvironmentFactory(null);
            var result = target.ValidEnvironments;
            result.Count.Should().Be(1);
            result.First().Should().Be("");
        }

        [Test]
        public void Create_Test()
        {
            var instance = new object();
            var target = new InstanceEnvironmentFactory(instance);
            var result = target.Create("");
            result.Should().BeSameAs(instance);
        }
    }
}
