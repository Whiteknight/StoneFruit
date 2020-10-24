﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Handler source for function delegates
    /// </summary>
    public class DelegateHandlerSource : IHandlerSource
    {
        private readonly VerbTrie<HandlerFactory> _handlers;

        public DelegateHandlerSource()
        {
            _handlers = new VerbTrie<HandlerFactory>();
        }

        public int Count => _handlers.Count;

        public IHandlerBase GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            var factory = _handlers.Get(arguments);
            if (factory == null)
                return null;
            return factory.Create(arguments, dispatcher);
        }

        public IEnumerable<IVerbInfo> GetAll() => _handlers.GetAll().Select(kvp => kvp.Value);

        public IVerbInfo GetByName(Verb verb) => _handlers.Get(verb);

        public DelegateHandlerSource Add(Verb verb, Action<IArguments, CommandDispatcher> act, string description = null, string usage = null, string group = null)
        {
            var factory = new SyncHandlerFactory(act, verb, description, usage, group);
            _handlers.Insert(verb, factory);
            return this;
        }

        public DelegateHandlerSource AddAsync(Verb verb, Func<IArguments, CommandDispatcher, Task> func, string description = null, string usage = null, string group = null)
        {
            _handlers.Insert(verb, new AsyncHandlerFactory(func, verb, description, usage, group));
            return this;
        }

        private abstract class HandlerFactory : IVerbInfo
        {
            protected HandlerFactory(Verb verb, string description, string usage, string group)
            {
                Verb = verb;
                Description = description ?? string.Empty;
                Usage = usage ?? Description;
                Group = group;
            }

            public Verb Verb { get; }
            public string Description { get; }
            public string Usage { get; }
            public string Group { get; }
            public bool ShouldShowInHelp => true;

            public abstract IHandlerBase Create(IArguments arguments, CommandDispatcher dispatcher);
        }

        private class SyncHandlerFactory : HandlerFactory
        {
            private readonly Action<IArguments, CommandDispatcher> _act;

            public SyncHandlerFactory(Action<IArguments, CommandDispatcher> act, Verb verb, string description, string usage, string group)
                : base(verb, description, usage, group)
            {
                _act = act;
            }


            public override IHandlerBase Create(IArguments arguments, CommandDispatcher dispatcher)
            {
                return new SyncHandler(_act, arguments, dispatcher);
            }
        }

        private class AsyncHandlerFactory : HandlerFactory
        {
            private readonly Func<IArguments, CommandDispatcher, Task> _func;

            public AsyncHandlerFactory(Func<IArguments, CommandDispatcher, Task> func, Verb verb, string description, string usage, string group)
                : base(verb, description, usage, group)
            {
                _func = func;
            }

            public override IHandlerBase Create(IArguments command, CommandDispatcher dispatcher)
            {
                return new AsyncHandler(_func, command, dispatcher);
            }
        }

        private class SyncHandler : IHandler
        {
            private readonly Action<IArguments, CommandDispatcher> _act;
            private readonly IArguments _arguments;
            private readonly CommandDispatcher _dispatcher;

            public SyncHandler(Action<IArguments, CommandDispatcher> act, IArguments arguments, CommandDispatcher dispatcher)
            {
                _act = act;
                _arguments = arguments;
                _dispatcher = dispatcher;
            }

            public void Execute()
            {
                _act(_arguments, _dispatcher);
            }
        }

        private class AsyncHandler : IAsyncHandler
        {
            private readonly Func<IArguments, CommandDispatcher, Task> _func;
            private readonly IArguments _arguments;
            private readonly CommandDispatcher _dispatcher;

            public AsyncHandler(Func<IArguments, CommandDispatcher, Task> func, IArguments arguments, CommandDispatcher dispatcher)
            {
                _func = func;
                _arguments = arguments;
                _dispatcher = dispatcher;
            }

            public Task ExecuteAsync(CancellationToken cancellation)
            {
                return _func(_arguments, _dispatcher);
            }
        }
    }
}
