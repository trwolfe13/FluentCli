﻿using System;

namespace Wolfe.FluentCli.Core.Build
{
    public interface ICommandBuilder
    {
        ICommandBuilder WithOptions<TArgs>(Action<IOptionsBuilder<TArgs>> options = null);
    }
}