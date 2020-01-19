using System;

namespace StoneFruit
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CommandNameAttribute : Attribute
    {
        public CommandNameAttribute(string command, bool showInHelp = true)
        {
            CommandName = command;
            ShowInHelp = showInHelp;
        }

        public string CommandName { get; }

        public bool ShowInHelp { get; }
    }
}