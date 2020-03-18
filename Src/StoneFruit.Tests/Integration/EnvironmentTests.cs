﻿using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Handlers;
using StoneFruit.Tests.Helpers;

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
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler), typeof(EnvironmentChangeHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseInstances(new Dictionary<string, object> {
                    { "A", new TestEnvironment("A")},
                    { "B", new TestEnvironment("B")}
                }))
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
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler), typeof(EnvironmentChangeHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new TestEnvironmentFactory()))
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
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestEnvironmentHandler), typeof(EnvironmentChangeHandler), typeof(EchoHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new TestEnvironmentFactory()))
                .Build();
            engine.RunInteractively("C");
            output.Lines.Count.Should().Be(4);
            output.Lines[3].Should().Be("C");
        }
    }
}