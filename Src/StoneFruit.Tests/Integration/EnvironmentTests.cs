﻿using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class EnvironmentTests
    {
        private class TestEnvironment
        {
            public string Name { get; }

            public TestEnvironment(string name)
            {
                Name = name;
            }
        }

        [Verb("test")]
        private class TestEnvironmentHandler : IHandler
        {
            private readonly TestEnvironment _env;
            private readonly IOutput _output;

            public TestEnvironmentHandler(TestEnvironment env, IOutput output)
            {
                _env = env;
                _output = output;
            }

            public void Execute()
            {
                _output.WriteLine(_env.Name);
            }
        }

        [Test]
        public void Instance_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseInstance(new TestEnvironment("Single")))
                .Build();
            engine.RunHeadless("test");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("Single");
        }

        [Test]
        public void Dictionary_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                // We need env-change handler, so we can select one on startup
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseInstances(new Dictionary<string, object> {
                    { "A", new TestEnvironment("A")},
                    { "B", new TestEnvironment("B")}
                }))
                .SetupEvents(e => { e.EnvironmentChanged.Clear(); })
                .Build();
            engine.RunHeadless("B test");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("B");
        }

        public class TestEnvironmentFactory : IEnvironmentFactory
        {
            public object Create(string name)
            {
                return new TestEnvironment(name);
            }

            public IReadOnlyCollection<string> ValidEnvironments => new[] { "A", "B", "C" };
        }

        [Test]
        public void Factory_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                // We need env-change handler, so we can select one on startup
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new TestEnvironmentFactory()))
                .SetupEvents(e => { e.EnvironmentChanged.Clear(); })
                .Build();
            engine.RunHeadless("B test");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("B");
        }

        [Test]
        public void Factory_Interactive()
        {
            var output = new TestOutput("test");
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new TestEnvironmentFactory()))
                // Clear out start/stop scripts so we don't have anything extra in output
                .SetupEvents(c =>
                {
                    c.EnvironmentChanged.Clear();
                    c.EngineStartInteractive.Clear();
                    //c.EngineStopInteractive.Clear();
                })
                .Build();
            engine.RunInteractively("C");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("C");
        }

        [Test]
        public void Factory_NotSet()
        {
            var output = new TestOutput("env -notset B", "test");
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new TestEnvironmentFactory()))
                .SetupEvents(c =>
                {
                    c.EngineStartInteractive.Clear();
                    c.EnvironmentChanged.Clear();
                })
                .Build();
            engine.RunInteractively();
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("B");
        }

        [Test]
        public void Factory_NotSet_AlreadySet()
        {
            var output = new TestOutput("env A", "env -notset B", "test");
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new TestEnvironmentFactory()))
                .SetupEvents(c =>
                {
                    c.EngineStartInteractive.Clear();
                    c.EnvironmentChanged.Clear();
                })
                .Build();
            engine.RunInteractively();
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("A");
        }

        [Test]
        public void Factory_SetByIndex()
        {
            var output = new TestOutput("env 2", "test");
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new TestEnvironmentFactory()))
                .SetupEvents(c =>
                {
                    c.EngineStartInteractive.Clear();
                    c.EnvironmentChanged.Clear();
                })
                .Build();
            engine.RunInteractively();
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("B");
        }

        [Test]
        public void Factory_List()
        {
            var output = new TestOutput("env -list");
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new TestEnvironmentFactory()))
                .SetupEvents(c =>
                {
                    c.EngineStartInteractive.Clear();
                    c.EnvironmentChanged.Clear();
                })
                .Build();
            engine.RunInteractively();
            output.Lines.Should().Contain("A");
            output.Lines.Should().Contain("B");
            output.Lines.Should().Contain("C");
        }

        [Test]
        public void Factory_Invalid()
        {
            var output = new TestOutput("env D");
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new TestEnvironmentFactory()))
                .SetupEvents(c =>
                {
                    c.EngineStartInteractive.Clear();
                    c.EnvironmentChanged.Clear();
                })
                .Build();
            engine.RunInteractively();
            output.Lines[0].Should().Be("Unknown environment 'D'");
        }

        [Test]
        public void Factory_Prompt_Name()
        {
            var output = new TestOutput("env", "B", "test");
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new TestEnvironmentFactory()))
                .SetupEvents(c =>
                {
                    c.EngineStartInteractive.Clear();
                    c.EnvironmentChanged.Clear();
                })
                .Build();
            engine.RunInteractively();
            output.Lines[output.Lines.Count - 1].Should().Be("B");
        }

        [Test]
        public void Factory_Prompt_Index()
        {
            var output = new TestOutput("env", "2", "test");
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new TestEnvironmentFactory()))
                .SetupEvents(c =>
                {
                    c.EngineStartInteractive.Clear();
                    c.EnvironmentChanged.Clear();
                })
                .Build();
            engine.RunInteractively();
            output.Lines[output.Lines.Count - 1].Should().Be("B");
        }
    }
}
