using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Environments;

namespace StoneFruit.Tests.Execution.Environments
{
    public class InstanceEnvironmentCollectionTests
    {
        //[Test]
        //public void IsValid_string_Tests()
        //{
        //    var target = new InstanceEnvironmentCollection(null);
        //    target.IsValid("").Should().BeTrue();
        //    target.IsValid("xxx").Should().BeFalse();
        //}

        [Test]
        public void IsValid_int_Tests()
        {
            var target = new InstanceEnvironmentCollection(null);
            target.IsValid(-1).Should().BeFalse();
            target.IsValid(0).Should().BeTrue();
            target.IsValid(1).Should().BeFalse();
            target.IsValid(2).Should().BeFalse();
        }
    }
}
