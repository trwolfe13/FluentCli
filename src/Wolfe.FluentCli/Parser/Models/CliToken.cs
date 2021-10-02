﻿namespace Wolfe.FluentCli.Parser.Models
{
    public record CliToken
    {
        public CliToken(CliTokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public CliTokenType Type { get; }
        public string Value { get; }

        public static readonly CliToken Eof = new(CliTokenType.Eof, null);
    }
}