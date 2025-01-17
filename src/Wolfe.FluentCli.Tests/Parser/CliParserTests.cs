﻿using System.Collections.Generic;
using Wolfe.FluentCli.Core.Internal;
using Wolfe.FluentCli.Parser;
using Wolfe.FluentCli.Parser.Definition;
using Xunit;

namespace Wolfe.FluentCli.Tests.Parser
{
    public class CliParserTests
    {
        private static readonly CliDefinition ComplexDefinition = new()
        {
            Unnamed = { AllowedValues = AllowedValues.Many },
            NamedArguments =
            {
                new() { LongName = "one", ShortName = "o", AllowedValues = AllowedValues.One },
                new() { LongName = "many", ShortName = "m", AllowedValues = AllowedValues.Many }
            },
            Commands =
            {
                new()
                {
                    Aliases = { "f", "foo" },
                    Unnamed = { AllowedValues = AllowedValues.One },
                    NamedArguments =
                    {
                        new() { LongName = "bar", ShortName = "b", AllowedValues = AllowedValues.Many }
                    },
                    Commands =
                    {
                        new()
                        {
                            Aliases = { "b", "bar" },
                            Unnamed = { AllowedValues = AllowedValues.Many }
                        }
                    }
                },
                new()
                {
                    Aliases = { "b", "bar" },
                    Unnamed = { AllowedValues = AllowedValues.None },
                    Commands =
                    {
                        new()
                        {
                            Aliases = { "f", "foo" },
                            Unnamed = { AllowedValues = AllowedValues.One }
                        }
                    }
                }
            }
        };

        [Fact]
        public void NestedCommands_ReturnsFirstAlias()
        {
            var scanner = new CliScanner("foo bar");
            var parser = new CliParser();
            var instruction = parser.Parse(scanner, ComplexDefinition);

            Assert.Equal(new List<string> { "f", "b" }, instruction.Commands);
        }

        [Fact]
        public void RootUnnamedArgs_ReturnsArgs()
        {
            var scanner = new CliScanner("random \"unnamed arguments\"");
            var parser = new CliParser();
            var result = parser.Parse(scanner, ComplexDefinition);

            Assert.Equal(new List<string>(), result.Commands);
            Assert.Equal(new List<string> { "random", "unnamed arguments" }, result.Unnamed.Values);
        }

        [Fact]
        public void CommandWithUnnamedArgs_ReturnsBoth()
        {
            var scanner = new CliScanner("foo combine");
            var parser = new CliParser();
            var instruction = parser.Parse(scanner, ComplexDefinition);

            Assert.Equal(new List<string> { "f" }, instruction.Commands);
            Assert.Equal(new List<string> { "combine" }, instruction.Unnamed.Values);
        }

        [Fact]
        public void NamedArgs_ReturnsArgs()
        {
            var scanner = new CliScanner("-m many args");
            var parser = new CliParser();
            var instruction = parser.Parse(scanner, ComplexDefinition);

            Assert.Equal(new List<string>(), instruction.Commands);
            Assert.Equal(new List<string>(), instruction.Unnamed.Values);
            Assert.Equal("many", instruction.Named[0].Name);
            Assert.Equal(new List<string> { "many", "args" }, instruction.Named[0].Values);
        }


        [Fact]
        public void AllParts_ReturnsAll()
        {
            var scanner = new CliScanner("foo test --bar arg1 arg2");
            var parser = new CliParser();
            var instruction = parser.Parse(scanner, ComplexDefinition);

            Assert.Equal(new List<string> { "f" }, instruction.Commands);
            Assert.Equal(new List<string> { "test" }, instruction.Unnamed.Values);
            Assert.Equal("bar", instruction.Named[0].Name);
            Assert.Equal(new List<string> { "arg1", "arg2" }, instruction.Named[0].Values);
        }
    }
}
