using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class TimeoutTests
    {
        //[Test]
        //public void AsyncTimeout_Test()
        //{
        //    var output = new TestOutput();
        //    var engine = new EngineBuilder()
        //        .SetupHandlers(h => h.UseHandlerTypes(typeof(TestHandler)))
        //        .SetupOutput(o => o.DoNotUseConsole().Add(output))
        //        .SetupEvents(c =>
        //        {
        //        })
        //        .SetupSettings(s => { 
        //            s.MaxExecuteTimeout = TimeSpan.FromSeconds(2);
        //        })
        //        .Build();
        //    engine.RunHeadless("test");
        //    // Give a pretty wide range here, we don't care exactly what the number is and
        //    // we don't want to throw false negatives if it took the threadpool a long 
        //    // time to spool up the task.
        //    output.Lines.Count.Should().BeInRange(1, 30);
        //}

        [Verb("test")]
        public class TestHandler : IAsyncHandler
        {
            private readonly IOutput _output;

            public TestHandler(IOutput output)
            {
                _output = output;
            }

            public async Task ExecuteAsync(CancellationToken token)
            {
                await Task.Run(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        _output.WriteLine(".");
                        Thread.Sleep(100);
                    }
                });
            }
        }
    }
}