using FluentAssertions;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class HelpTests
    {
        [Test]
        public void Help_Builtins()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            engine.RunHeadless("help");
            // 'exit' and 'help' are always visible
            output.Lines.Should().Contain("exit");
            output.Lines.Should().Contain("help");

            // 'echo' is hidden by default
            output.Lines.Should().NotContain("echo");
        }

        [Test]
        public void Help_Builtins_ShowAll()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            engine.RunHeadless("help -showall");
            output.Lines.Should().Contain("exit");
            output.Lines.Should().Contain("help");
            output.Lines.Should().Contain("echo");
        }

        private class TestInstanceMethodsAttrs
        {
            [Description("Test1Description")]
            [Usage("Test1Usage")]
            [Group("Test1Group")]
            public void Test1()
            {
            }

            [Description("Test2Description")]
            [Usage("Test2Usage")]
            [Group("Test2Group")]
            public void Test2()
            {
            }
        }

        [Test]
        public void Help_InstanceMethodAttributes_GroupDescription()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .UsePublicMethodsAsHandlers(new TestInstanceMethodsAttrs())
                )
            );
            engine.RunHeadless("help");
            output.Lines.Should().Contain("Test1Description");
            output.Lines.Should().Contain("Test1Group");
            output.Lines.Should().Contain("Test2Description");
            output.Lines.Should().Contain("Test2Group");
        }

        [Test]
        public void Help_InstanceMethodAttributes_Usage()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .UsePublicMethodsAsHandlers(new TestInstanceMethodsAttrs())
                )
            );
            engine.RunHeadless("help test 1");
            output.Lines.Should().Contain("Test1Usage");
        }

        [Description("AttrDescription")]
        [Usage("AttrUsage")]
        [Group("AttrGroup")]
        private class AttributesHelpHandler : IHandler
        {
            public void Execute()
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void Help_HandlerAttributes_GroupDescription()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .Add("test-attrs", new AttributesHelpHandler())
                )
            );
            engine.RunHeadless("help");
            output.Lines.Should().Contain("AttrDescription");
            output.Lines.Should().Contain("AttrGroup");
        }

        [Test]
        public void Help_HandlerAttributes_Usage()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .Add("test-attrs", new AttributesHelpHandler())
                )
            );
            engine.RunHeadless("help test-attrs");
            output.Lines.Should().Contain("AttrUsage");
        }

        [Test]
        public void Help_HandlerAttributes_OverrideGroupDescription()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .Add("test-attrs", new AttributesHelpHandler(), description: "OverrideDescription", group: "OverrideGroup")
                )
            );
            engine.RunHeadless("help");
            output.Lines.Should().Contain("OverrideDescription");
            output.Lines.Should().Contain("OverrideGroup");
        }

        [Test]
        public void Help_HandlerAttributes_OverrideUsage()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .Add("test-attrs", new AttributesHelpHandler(), usage: "OverrideUsage")
                )
            );
            engine.RunHeadless("help test-attrs");
            output.Lines.Should().Contain("OverrideUsage");
        }

        [System.ComponentModel.Description("AttrDescription")]
        [System.ComponentModel.Category("AttrGroup")]
        private class ComponentModelAttributesHelpHandler : IHandler
        {
            public void Execute()
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void Help_ComponentModelAttributes_GroupDescription()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .Add("test-attrs", new ComponentModelAttributesHelpHandler())
                )
            );
            engine.RunHeadless("help");
            output.Lines.Should().Contain("AttrDescription");
            output.Lines.Should().Contain("AttrGroup");
        }

        [System.ComponentModel.Description("BrowseableFalse")]
        [System.ComponentModel.Browsable(false)]
        private class ComponentModelBrowseableAttributeFalseHandler : IHandler
        {
            public void Execute()
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void Help_ComponentModelAttributes_Browsable_False()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .Add("test-attrs", new ComponentModelBrowseableAttributeFalseHandler())
                )
            );
            engine.RunHeadless("help");
            output.Lines.Should().NotContain("BrowseableFalse");
        }

        [System.ComponentModel.Description("BrowseableTrue")]
        [System.ComponentModel.Browsable(true)]
        private class ComponentModelBrowseableAttributeTrueHandler : IHandler
        {
            public void Execute()
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void Help_ComponentModelAttributes_Browsable_True()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .Add("test-attrs", new ComponentModelBrowseableAttributeTrueHandler())
                )
            );
            engine.RunHeadless("help");
            output.Lines.Should().Contain("BrowseableTrue");
        }

        [Verb("hidden-handler", Hide = true)]
        private class VerbAttributeHideHandler : IHandler
        {
            public void Execute()
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void Help_VerbAttribute_Hide_True()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .Add("hidden-handler", new VerbAttributeHideHandler())
                )
            );
            engine.RunHeadless("help");
            output.Lines.Should().NotContain("hidden-handler");
        }

        [Verb("visible-handler", Hide = false)]
        private class VerbAttributeShowHandler : IHandler
        {
            public void Execute()
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void Help_VerbAttribute_Hide_False()
        {
            var output = new TestOutput();
            var engine = EngineBuilder.Build(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .Add("visible-handler", new VerbAttributeShowHandler())
                )
            );
            engine.RunHeadless("help");
            output.Lines.Should().Contain("visible-handler");
        }
    }
}
