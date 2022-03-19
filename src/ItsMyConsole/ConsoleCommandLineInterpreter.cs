using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ItsMyConsole
{
    /// <summary>
    /// Framework pour une application console qui facilite l'implémentation d'interpréteur de commande
    /// </summary>
    public class ConsoleCommandLineInterpreter
    {
        private readonly ConsoleOptions _options;
        private readonly Dictionary<CommandPattern, object> _commandPatternCallbacks;

        /// <summary>
        /// Framework pour une application console qui facilite l'implémentation d'interpréteur de commande
        /// </summary>
        public ConsoleCommandLineInterpreter() {
            _commandPatternCallbacks = new Dictionary<CommandPattern, object>();
            _options = CreateDefaultOptions();
        }

        private static ConsoleOptions CreateDefaultOptions() {
            return new ConsoleOptions {
                Prompt = ">",
                LineBreakBetweenCommands = false,
                HeaderText = "",
                TrimCommand = true,
                DefaultCommandRegexOptions = RegexOptions.None,
                AddStartAndEndCommandPatternAuto = false,
                HeaderTextColor = Console.ForegroundColor,
                PromptColor = Console.ForegroundColor,
                CommandColor = Console.ForegroundColor
            };
        }

        /// <summary>
        /// Configuration des options d'affichages de la console
        /// </summary>
        /// <param name="configureOptions">Les options d'affichage de la console</param>
        public void Configure(Action<ConsoleOptions> configureOptions) {
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));
            configureOptions.Invoke(_options);
        }

        /// <summary>
        /// L'ajout de l'interprétation d'une ligne de commande et de son exécution
        /// </summary>
        /// <param name="pattern">L'expression régulière d'interprétation de la ligne de commande</param>
        /// <param name="callback">L'exécution de la ligne de commande</param>
        public void AddCommand(string pattern, Action<CommandTools> callback) {
            AddPatternAndCallback(pattern, _options.DefaultCommandRegexOptions, callback);
        }

        /// <summary>
        /// L'ajout de l'interprétation d'une ligne de commande et de son exécution
        /// </summary>
        /// <param name="pattern">L'expression régulière d'interprétation de la ligne de commande</param>
        /// <param name="callback">L'exécution de la ligne de commande</param>
        public void AddCommand(string pattern, Func<CommandTools, Task> callback) {
            AddPatternAndCallback(pattern, _options.DefaultCommandRegexOptions, callback);
        }

        /// <summary>
        /// L'ajout de l'interprétation d'une ligne de commande et de son exécution
        /// </summary>
        /// <param name="pattern">L'expression régulière d'interprétation de la ligne de commande</param>
        /// <param name="regexOptions">Combinaison d'opérations de bits des valeurs d'énumération qui fournissent des options pour la correspondance</param>
        /// <param name="callback">L'exécution de la ligne de commande</param>
        public void AddCommand(string pattern, RegexOptions regexOptions, Action<CommandTools> callback) {
            AddPatternAndCallback(pattern, regexOptions, callback);
        }

        /// <summary>
        /// L'ajout de l'interprétation d'une ligne de commande et de son exécution
        /// </summary>
        /// <param name="pattern">L'expression régulière d'interprétation de la ligne de commande</param>
        /// <param name="regexOptions">Combinaison d'opérations de bits des valeurs d'énumération qui fournissent des options pour la correspondance</param>
        /// <param name="callback">L'exécution de la ligne de commande</param>
        public void AddCommand(string pattern, RegexOptions regexOptions, Func<CommandTools, Task> callback) {
            AddPatternAndCallback(pattern, regexOptions, callback);
        }

        private void AddPatternAndCallback(string pattern, RegexOptions regexOptions, object callback) {
            ThrowIfPatternAndCallbackInvalid(pattern, callback);
            pattern = UpdatePattern(pattern);
            ThrowIfPatternExists(pattern);
            _commandPatternCallbacks.Add(new CommandPattern {
                                             Pattern = pattern,
                                             RegexOptions = regexOptions
                                         }, callback);
        }

        private static void ThrowIfPatternAndCallbackInvalid(string pattern, object command) {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));
            if (command == null)
                throw new ArgumentNullException(nameof(command));
        }

        private string UpdatePattern(string pattern) {
            bool startExists = pattern.StartsWith("^");
            bool endExists = pattern.EndsWith("$");
            bool addStartEndAuto = _options.AddStartAndEndCommandPatternAuto && !startExists && !endExists;
            return addStartEndAuto ? $"^{pattern}$" : pattern;
        }

        private void ThrowIfPatternExists(string pattern) {
            if (_commandPatternCallbacks.Keys.Any(k => k.Pattern == pattern))
                throw new ArgumentException("Pattern déjà présent", nameof(pattern));
        }

        /// <summary>
        /// Lance de la console en attente des lignes de commandes
        /// </summary>
        public async Task RunAsync() {
            ShowHeader();
            string command = WaitNextCommand();
            while (!IsExitCommand(command)) {
                await RunCommandAsync(command);
                ShowLineBreakBetweenCommands();
                command = WaitNextCommand();
            }
        }

        private void ShowHeader() {
            if (!string.IsNullOrEmpty(_options.HeaderText))
                ConsoleWriteLineColor(_options.HeaderText, _options.HeaderTextColor);
        }

        private static void ConsoleWriteLineColor(string text, ConsoleColor consoleColor) {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(text);
            Console.ForegroundColor = currentColor;
        }

        private string WaitNextCommand() {
            string command;
            do {
                PromptCommand();
                command = ConsoleReadLineColor(_options.CommandColor) ?? "";
                command = _options.TrimCommand ? command.Trim() : command;
            } while (string.IsNullOrEmpty(command));
            return command;
        }

        private void PromptCommand() {
            ConsoleWriteColor(_options.Prompt, _options.PromptColor);
        }

        private static void ConsoleWriteColor(string text, ConsoleColor consoleColor) {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            Console.Write(text);
            Console.ForegroundColor = currentColor;
        }

        private static string ConsoleReadLineColor(ConsoleColor consoleColor) {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            string readLine = Console.ReadLine();
            Console.ForegroundColor = currentColor;
            return readLine;
        }

        private static bool IsExitCommand(string command) {
            return command.Equals("exit", StringComparison.OrdinalIgnoreCase);
        }

        private async Task RunCommandAsync(string command) {
            try {
                foreach ((CommandPattern commandPattern, object callback) in _commandPatternCallbacks.Select(x => (x.Key, x.Value))) {
                    Match match = Regex.Match(command, commandPattern.Pattern, commandPattern.RegexOptions);
                    if (match.Success) {
                        CommandTools tools = CreateTools(command, match);
                        switch (callback) {
                            case Func<CommandTools, Task> funcAsync:
                                await funcAsync(tools);
                                break;
                            case Action<CommandTools> action:
                                action(tools);
                                break;
                        }
                        return;
                    }
                }
                throw new Exception("Commande non trouvée");
            }
            catch (Exception ex) {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        private static CommandTools CreateTools(string command, Match commandMatch) {
            return new CommandTools {
                Command = command,
                CommandMatch = commandMatch,
                CommandArgs = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
            };
        }

        private void ShowLineBreakBetweenCommands() {
            if (_options.LineBreakBetweenCommands)
                Console.WriteLine();
        }
    }
}
