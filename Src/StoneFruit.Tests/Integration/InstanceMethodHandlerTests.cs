﻿using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Handlers;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class InstanceMethodHandlerTests
    {
        public class MyObject1
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

        [Test]
        public void A_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UsePublicMethodsAsHandlers(new MyObject1()))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test a");
            output.Lines[0].Should().Be("A");
        }

        [Test]
        public void A_LowerCaseVerbExtractor()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UsePublicMethodsAsHandlers(new MyObject1(), verbExtractor: new ToLowerNameVerbExtractor()))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("testa");
            output.Lines[0].Should().Be("A");
        }

        [Test]
        public void A_CamelToSpinalVerbExtractor()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UsePublicMethodsAsHandlers(new MyObject1(), verbExtractor: new CamelCaseToSpinalCaseVerbExtractor()))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test-a");
            output.Lines[0].Should().Be("A");
        }

        [Test]
        public void B_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UsePublicMethodsAsHandlers(new MyObject1()))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test b name=x");
            output.Lines[0].Should().Be("B: x");
        }

        [Test]
        public void C_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UsePublicMethodsAsHandlers(new MyObject1()))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test c");
            output.Lines[0].Should().Be("C");
        }

        [Test]
        public void InstanceMethods_Help()
        {
            var output = new TestOutput("help");
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UsePublicMethodsAsHandlers(new MyObject1()))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunInteractively();
            output.Lines.Should().Contain("test a");
            output.Lines.Should().Contain("test b");
            output.Lines.Should().Contain("test c");
        }

        public class MyObject2
        {
            [Verb("aaa")]
            public void TestA(IOutput output)
            {
                output.WriteLine("A");
            }
        }

        [Test]
        public void A_VerbAttribute()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UsePublicMethodsAsHandlers(new MyObject2()))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("aaa");
            output.Lines[0].Should().Be("A");
        }
    }
}