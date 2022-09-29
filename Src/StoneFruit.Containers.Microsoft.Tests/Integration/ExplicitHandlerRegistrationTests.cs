using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Containers.Microsoft.Tests.Integration
{
    public class ExplicitHandlerRegistrationTests
    {
        [Verb("test-resolve")]
        private class TestResolveHandler : IHandler
        {
            private readonly IOutput _output;

            public TestResolveHandler(IOutput output)
            {
                _output = output;
            }

            public void Execute()
            {
                _output.WriteLine("TEST");
            }
        }

        [Test]
        public void RegisterType_Test()
        {
            var output = new TestOutput();
            var services = new ServiceCollection();
            services.AddTransient<TestResolveHandler>();
            IServiceProvider provider = null;
            services.SetupEngine<TestEnvironment>(b => b
                    .SetupOutput(o => o.DoNotUseConsole().Add(output))
                    .SetupEnvironments(e => e.UseInstance(new TestEnvironment("Single"))),
                () => provider
            );
            provider = services.BuildServiceProvider();
            var engine = provider.GetService<Engine>();
            engine.RunHeadless("test-resolve");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("TEST");
        }

        [Test]
        public void RegisterInstance_Test()
        {
            var output = new TestOutput();
            var services = new ServiceCollection();
            var instance = new TestResolveHandler(output);
            services.AddSingleton(instance);
            IServiceProvider provider = null;
            services.SetupEngine<TestEnvironment>(b => b
                    .SetupOutput(o => o.DoNotUseConsole().Add(output))
                    .SetupEnvironments(e => e.UseInstance(new TestEnvironment("Single"))),
                () => provider
            );
            provider = services.BuildServiceProvider();
            var engine = provider.GetService<Engine>();
            engine.RunHeadless("test-resolve");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("TEST");
        }

        [Test]
        public void RegisterFactory_Test()
        {
            var output = new TestOutput();
            var services = new ServiceCollection();
            services.AddTransient(p => new TestResolveHandler(output));
            IServiceProvider provider = null;
            services.SetupEngine<TestEnvironment>(b => b
                    .SetupOutput(o => o.DoNotUseConsole().Add(output))
                    .SetupEnvironments(e => e.UseInstance(new TestEnvironment("Single"))),
                () => provider
            );
            provider = services.BuildServiceProvider();
            var engine = provider.GetService<Engine>();
            engine.RunHeadless("test-resolve");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("TEST");
        }
    }
}
