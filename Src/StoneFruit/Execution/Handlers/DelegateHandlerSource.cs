﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Handler source for function delegates
    /// </summary>
    public class DelegateHandlerSource : IHandlerSource
    {
        private readonly Dictionary<string, HandlerFactory> _handlers;

        public DelegateHandlerSource()
        {
            _handlers = new Dictionary<string, HandlerFactory>();
        }

        public int Count => _handlers.Count;

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
            => _handlers.ContainsKey(command.Verb) ? _handlers[command.Verb].Create(command, dispatcher) : null;

        public IEnumerable<IVerbInfo> GetAll() => _handlers.Values;

        public IVerbInfo GetByName(string name) => _handlers.ContainsKey(name) ? _handlers[name] : null;

        public DelegateHandlerSource Add(string verb, Action<Command, CommandDispatcher> act, string description = null, string usage = null, string group = null)
        {
            _handlers.Add(verb, new SyncHandlerFactory(act, verb, description, usage, group));
            return this;
        }

        public DelegateHandlerSource AddAsync(string verb, Func<Command, CommandDispatcher, Task> func, string description = null, string usage = null, string group = null)
        {
            _handlers.Add(verb, new AsyncHandlerFactory(func, verb, description, usage, group));
            return this;
        }

        private abstract class HandlerFactory : IVerbInfo
        {
            protected HandlerFactory(string verb, string description, string usage, string group)
            {
                Verb = verb;
                Description = description ?? string.Empty;
                Usage = usage ?? Description;
                Group = group;
            }

            public string Verb { get; }
            public string Description { get; }
            public string Usage { get; }
            public string Group { get; }
            public bool ShouldShowInHelp => true;

            public abstract IHandlerBase Create(Command command, CommandDispatcher dispatcher);
        }

        private class SyncHandlerFactory : HandlerFactory
        {
            private readonly Action<Command, CommandDispatcher> _act;

            public SyncHandlerFactory(Action<Command, CommandDispatcher> act, string verb, string description, string usage, string group)
                : base(verb, description, usage, group)
            {
                _act = act;
            }


            public override IHandlerBase Create(Command command, CommandDispatcher dispatcher)
            {
                return new SyncHandler(_act, command, dispatcher);
            }
        }

        private class AsyncHandlerFactory : HandlerFactory
        {
            private readonly Func<Command, CommandDispatcher, Task> _func;

            public AsyncHandlerFactory(Func<Command, CommandDispatcher, Task> func, string verb, string description, string usage, string group)
                : base(verb, description, usage, group)
            {
                _func = func;
            }

            public override IHandlerBase Create(Command command, CommandDispatcher dispatcher)
            {
                return new AsyncHandler(_func, command, dispatcher);
            }
        }

        private class SyncHandler : IHandler
        {
            private readonly Action<Command, CommandDispatcher> _act;
            private readonly Command _command;
            private readonly CommandDispatcher _dispatcher;

            public SyncHandler(Action<Command, CommandDispatcher> act, Command command, CommandDispatcher dispatcher)
            {
                _act = act;
                _command = command;
                _dispatcher = dispatcher;
            }

            public void Execute()
            {
                _act(_command, _dispatcher);
            }
        }

        private class AsyncHandler : IAsyncHandler
        {
            private readonly Func<Command, CommandDispatcher, Task> _func;
            private readonly Command _command;
            private readonly CommandDispatcher _dispatcher;

            public AsyncHandler(Func<Command, CommandDispatcher, Task> func, Command command, CommandDispatcher dispatcher)
            {
                _func = func;
                _command = command;
                _dispatcher = dispatcher;
            }

            public Task ExecuteAsync(CancellationToken cancellation)
            {
                return _func(_command, _dispatcher);
            }
        }
    }
}
