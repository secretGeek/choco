﻿namespace chocolatey.infrastructure.configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using app.configuration;
    using commandline;

    public static class ConfigurationOptions
    {
        private static readonly OptionSet _optionSet = new OptionSet();

        /// <summary>
        /// Parses arguments and updates the configuration
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="configuration">The configuration</param>
        /// <param name="setOptions">The set options.</param>
        /// <param name="helpMessage">The help message.</param>
        public static void parse_arguments_and_update_configuration(ICollection<string> args,
                                                                    IConfigurationSettings configuration,
                                                                    Action<OptionSet> setOptions,
                                                                    Action<IList<string>> afterParse,
                                                                    Action helpMessage)
        {
            IList<string> unparsedArguments = new List<string>();

            // add help only once
            if (_optionSet.Count == 0)
            {
                _optionSet
                    .Add("?|help|h",
                         "Prints out the options.",
                         option => configuration.HelpRequested = option != null);
            }

            if (setOptions != null)
            {
                setOptions(_optionSet);
            }

            try
            {
                unparsedArguments = _optionSet.Parse(args);
            }
            catch (OptionException)
            {
                show_help(_optionSet, helpMessage);
            }

            // the command argument
            if (string.IsNullOrWhiteSpace(configuration.CommandName) && unparsedArguments.Contains(args.FirstOrDefault()))
            {
                var commandName = args.FirstOrDefault();
                if (!Regex.IsMatch(commandName, @"^[-\/+]"))
                {
                    configuration.CommandName = commandName;
                }
                else
                {
                    configuration.HelpRequested = true;
                }
            }

            if (afterParse != null)
            {
                afterParse(unparsedArguments);
            }

            if (configuration.HelpRequested) show_help(_optionSet, helpMessage);
        }

        /// <summary>
        /// Shows the help menu and prints the options
        /// </summary>
        /// <param name="optionSet">The option_set.</param>
        private static void show_help(OptionSet optionSet, Action helpMessage)
        {
            if (helpMessage != null)
            {
                helpMessage.Invoke();
            }

            optionSet.WriteOptionDescriptions(Console.Error);
        }
    }
}