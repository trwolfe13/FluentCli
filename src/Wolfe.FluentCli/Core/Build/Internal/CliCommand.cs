﻿using System;
using System.Collections.Generic;

namespace Wolfe.FluentCli.Core.Build.Internal
{
    internal class CliCommand
    {
        public Type Handler { get; init; }
        public CliOptions Options { get; init; } = new();
        public List<CliNamedCommand> SubCommands { get; init; } = new();
    }
}