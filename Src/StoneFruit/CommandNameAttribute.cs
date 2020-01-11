using System;

namespace StoneFruit
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CommandNameAttribute : Attribute
    {
        public string CommandName { get; }

        public CommandNameAttribute(string command)
        {
            CommandName = command;
        }
    }
}