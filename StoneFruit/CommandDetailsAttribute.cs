using System;

namespace StoneFruit
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CommandDetailsAttribute : Attribute
    {
        public string CommandName { get; }
        public string Description { get; }
        public string[] Arguments { get; }

        public CommandDetailsAttribute(string command)
        {
            CommandName = command;
        }

        public CommandDetailsAttribute(string command, string description, params string[] argNames)
        {
            CommandName = command;
            Description = description;
            Arguments = argNames;
        }
    }
}