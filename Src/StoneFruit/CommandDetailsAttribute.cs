using System;

namespace StoneFruit
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CommandDetailsAttribute : Attribute
    {
        public string CommandName { get; }

        public CommandDetailsAttribute(string command)
        {
            CommandName = command;
        }
    }
}