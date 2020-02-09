using System;
using StoneFruit.Utility;

namespace StoneFruit
{
    /// <summary>
    /// Abstraction for output
    /// </summary>
    public interface IOutput
    {
        IOutput Color(Brush brush);

        IOutput WriteLine();
        IOutput WriteLine(string line);

        IOutput Write(string str);

        string Prompt(string prompt, bool mustProvide = true, bool keepHistory = true);

        // TODO: We need some kind of mechanism to synchronize in case we have multiple threads
        // writing complex multi-color or multi-line outputs without smashing each other. Some kind of
        // transaction abstraction, or a global output lock, or something will help here.
    }

    public static class OutputExtensions
    {
        public static IOutput WriteLineFormat(this IOutput output, string fmt, params object[] args)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNullOrEmpty(fmt, nameof(fmt));
            var line = string.Format(fmt, args);
            return output.WriteLine(line);
        }

        public static IOutput WriteFormat(this IOutput output, string fmt, params object[] args)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNullOrEmpty(fmt, nameof(fmt));
            var line = string.Format(fmt, args);
            return output.Write(line);
        }

        public static IOutput Color(this IOutput output, ConsoleColor color)
        {
            return output.Color((Brush) color);
        }
    }
}