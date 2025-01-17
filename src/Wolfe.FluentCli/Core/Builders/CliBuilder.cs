﻿using System;
using System.Collections.Generic;
using Wolfe.FluentCli.Commands;
using Wolfe.FluentCli.Core.Internal;
using Wolfe.FluentCli.Parser;

namespace Wolfe.FluentCli.Core.Builders
{
    internal class CliBuilder : ICliBuilder
    {
        private CommandBuilder _builder;
        private readonly List<CliNamedCommand> _subCommands = new();
        private ServiceProvider _serviceProvider = Activator.CreateInstance;

        public ICliBuilder WithServiceProvider(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            return this;
        }

        public ICliBuilder WithDefault<THandler, TArgs>() =>
            WithDefault<THandler>(command => command.WithArguments<TArgs>());

        public ICliBuilder WithDefault<THandler>(Action<ICommandBuilder> command = null)
        {
            var builder = new CommandBuilder("", typeof(THandler));
            command?.Invoke(builder);
            _builder = builder;
            return this;
        }

        public ICliBuilder AddCommand(string name, Action<INamedCommandBuilder> command) =>
            AddCommand<NullCommand>(name, command);

        public ICliBuilder AddCommand<THandler, TArgs>(string name) =>
            AddCommand<THandler>(name, command => command.WithArguments<TArgs>());

        public ICliBuilder AddCommand<THandler>(string name, Action<INamedCommandBuilder> command)
        {
            var cmd = BuildCommand<THandler>(name, command);
            _subCommands.Add(cmd);
            return this;
        }

        public IFluentCli Build()
        {
            var defaultBuilder = _builder ?? new CommandBuilder();
            var defaultCommand = defaultBuilder.BuildCommand();
            defaultCommand.Commands.AddRange(_subCommands);

            var parser = new CliParser();
            return new FluentCli(_serviceProvider, parser, defaultCommand);
        }

        private static CliNamedCommand BuildCommand<THandler>(string name, Action<INamedCommandBuilder> command)
        {
            var builder = new CommandBuilder(name, typeof(THandler));
            command?.Invoke(builder);
            return builder.BuildNamedCommand();
        }
    }
}
