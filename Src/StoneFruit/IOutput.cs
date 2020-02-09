using System;
using StoneFruit.Utility;

namespace StoneFruit
{
    /// <summary>
    /// Abstraction for output
    /// </summary>
    public interface IOutput
    {
        /// <summary>
        /// Get a new output using the given color palette. If the output does not support
        /// color this will be a no-op
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        IOutput Color(Brush brush);

        /// <summary>
        /// Write a linebreak to the output
        /// </summary>
        /// <returns></returns>
        IOutput WriteLine();

        /// <summary>
        /// Write the given text followed by a line break to the output
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        IOutput WriteLine(string line);

        /// <summary>
        /// Write the given text to the output
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        IOutput Write(string str);

        /// <summary>
        /// Show a prompt to the user to request input. For non-interactive outputs, this
        /// will be a no-op
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="mustProvide"></param>
        /// <param name="keepHistory"></param>
        /// <returns></returns>
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