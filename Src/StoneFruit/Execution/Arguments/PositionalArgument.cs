﻿namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// An argument which is defined by it's order in the list, not by name
    /// </summary>
    public class PositionalArgument : IArgument
    {
        public PositionalArgument(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public bool Consumed { get; private set; }

        public IArgument MarkConsumed()
        {
            Consumed = true;
            return this;
        }
    }
}