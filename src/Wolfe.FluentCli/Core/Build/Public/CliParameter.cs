﻿namespace Wolfe.FluentCli.Core.Build.Public
{
    public class CliParameter
    {
        public string ShortName { get; init; }
        public string LongName { get; init; }
        public bool Required { get; init; }
    }
}