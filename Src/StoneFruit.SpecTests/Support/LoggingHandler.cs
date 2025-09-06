using Microsoft.Extensions.Logging;

#pragma warning disable CA2254 // Template should be a static expression

namespace StoneFruit.SpecTests.Support;

public class LoggingHandler : IHandler
{
    private readonly ILogger<LoggingHandler> _logger;

    public LoggingHandler(ILogger<LoggingHandler> logger)
    {
        _logger = logger;
    }

    public void Execute(IArguments arguments, HandlerContext context)
    {
        var level = arguments.Shift().MarkConsumed().As(s => Enum.Parse<LogLevel>(s, true));
        var msg = string.Join(" ", arguments.GetUnconsumed());

        _logger.Log(level, msg);
    }
}
