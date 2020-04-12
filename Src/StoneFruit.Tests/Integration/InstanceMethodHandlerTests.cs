using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class InstanceMethodHandlerTests
    {
        public class MyObject
        {
            public void TestA(IOutput output)
            {
                output.WriteLine("A");
            }

            public void TestB(IOutput output, string name)
            {
                output.WriteLine($"B: {name}");
            }

            public Task TestC(IOutput output)
            {
                output.WriteLine("C");
                return Task.CompletedTask;
            }
        }

        //[Test]
        //public void A_Test()
        //{
        //    var output = new TestOutput();
        //    var engine = new EngineBuilder()
        //        .SetupHandlers(h => h.UsePublicMethodsAsHandlers(new MyObject()))
        //        .SetupOutput(o => o.DoNotUseConsole().Add(output))
        //        .Build();
        //    engine.RunHeadless("testa");
        //    output.Lines[0].Should().Be("A");
        //}

        //[Test]
        //public void B_Test()
        //{
        //    var output = new TestOutput();
        //    var engine = new EngineBuilder()
        //        .SetupHandlers(h => h.UsePublicMethodsAsHandlers(new MyObject()))
        //        .SetupOutput(o => o.DoNotUseConsole().Add(output))
        //        .Build();
        //    engine.RunHeadless("testb name=x");
        //    output.Lines[0].Should().Be("B: x");
        //}

        //[Test]
        //public void C_Test()
        //{
        //    var output = new TestOutput();
        //    var engine = new EngineBuilder()
        //        .SetupHandlers(h => h.UsePublicMethodsAsHandlers(new MyObject()))
        //        .SetupOutput(o => o.DoNotUseConsole().Add(output))
        //        .Build();
        //    engine.RunHeadless("testc");
        //    output.Lines[0].Should().Be("C");
        //}
    }
}