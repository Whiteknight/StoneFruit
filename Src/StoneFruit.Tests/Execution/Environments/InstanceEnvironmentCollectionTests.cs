using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Environments;

namespace StoneFruit.Tests.Execution.Environments
{
    public class InstanceEnvironmentCollectionTests
    {
        [Test]
        public void IsValid_string_Tests()
        {
            var target = new InstanceEnvironmentCollection(null);
            target.IsValid("").Should().BeTrue();
            target.IsValid("xxx").Should().BeFalse();
        }

        [Test]
        public void IsValid_int_Tests()
        {
            var target = new InstanceEnvironmentCollection(null);
            target.IsValid(0).Should().BeFalse();
            target.IsValid(1).Should().BeTrue();
            target.IsValid(2).Should().BeFalse();
        }

        [Test]
        public void Get_string_Tests()
        {
            var instance = new object();
            var target = new InstanceEnvironmentCollection(instance);
            var result = target.Get("");
            result.Should().BeSameAs(instance);
        }

        [Test]
        public void Get_int_Tests()
        {
            var instance = new object();
            var target = new InstanceEnvironmentCollection(instance);
            var result = target.Get(1);
            result.Should().BeSameAs(instance);
        }

        [Test]
        public void GetName_Tests()
        {
            var target = new InstanceEnvironmentCollection(null);
            var result = target.GetName(1);
            result.Should().Be("");
        }
    }
}
