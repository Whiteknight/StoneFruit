using System;

namespace StoneFruit
{
    /// <summary>
    /// Specify a custom verb to use if the correct verb cannot be automatically determined
    /// from the class name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class VerbAttribute : Attribute
    {
        public VerbAttribute(string command, bool showInHelp = true)
        {
            CommandName = command;
            ShowInHelp = showInHelp;
        }

        public string CommandName { get; }

        public bool ShowInHelp { get; }
    }
}