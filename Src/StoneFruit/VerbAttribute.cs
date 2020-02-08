using System;

namespace StoneFruit
{
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