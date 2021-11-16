using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ItsMyConsole
{
    public class ConsoleCommandLineInterpreter
    {
        private ConsoleOptions _options;
        private Action<ConsoleOptions> _configureOptions;
        private readonly Dictionary<CommandPattern, object> _commandPatternCallbacks;
        private readonly List<AzureDevOpsServer> _azureDevOpsServers;

        public ConsoleCommandLineInterpreter() {
            _commandPatternCallbacks = new Dictionary<CommandPattern, object>();
            _azureDevOpsServers = new List<AzureDevOpsServer>();
        }

        /// <summary>
        /// Configuration des options d'affichages de la console
        /// </summary>
        /// <param name="configureOptions">Les options d'affichage de la console</param>
        public void Configure(Action<ConsoleOptions> configureOptions) {
            _configureOptions = configureOptions;
        }

        /// <summary>
        /// Configuration d'un serveur Azure Dev Ops pour son utilisation pendant l'exécution d'une ligne de commande
        /// </summary>
        /// <param name="azureDevOpsServer">Les informations d'un serveur Azure Dev Ops</param>
        public void AddAzureDevOpsServer(AzureDevOpsServer azureDevOpsServer) {
            if (azureDevOpsServer == null)
                throw new ArgumentNullException(nameof(azureDevOpsServer));
            if (_azureDevOpsServers.Any(a => a.Name == azureDevOpsServer.Name))
                throw new ArgumentException("Nom déjà présent", nameof(azureDevOpsServer.Name));
            if (string.IsNullOrWhiteSpace(azureDevOpsServer.Url))
                throw new ArgumentException("Url obligatoire", nameof(azureDevOpsServer.Url));
            if (string.IsNullOrWhiteSpace(azureDevOpsServer.PersonalAccessToken))
                throw new ArgumentException("Token personnel obligatoire", nameof(azureDevOpsServer.PersonalAccessToken));
            _azureDevOpsServers.Add(azureDevOpsServer);
        }

        /// <summary>
        /// L'ajout de l'interprétation d'une ligne de commande et de son exécution
        /// </summary>
        /// <param name="pattern">L'expression réguliére d'interprétation de la ligne de commande</param>
        /// <param name="callback">L'exécution de la ligne de commande</param>
        public void AddCommand(string pattern, Action<CommandTools> callback) {
            AddPatternAndCallback(pattern, RegexOptions.None, callback);
        }

        /// <summary>
        /// L'ajout de l'interprétation d'une ligne de commande et de son exécution
        /// </summary>
        /// <param name="pattern">L'expression réguliére d'interprétation de la ligne de commande</param>
        /// <param name="callback">L'exécution de la ligne de commande</param>
        public void AddCommand(string pattern, Func<CommandTools, Task> callback) {
            AddPatternAndCallback(pattern, RegexOptions.None, callback);
        }

        /// <summary>
        /// L'ajout de l'interprétation d'une ligne de commande et de son exécution
        /// </summary>
        /// <param name="pattern">L'expression réguliére d'interprétation de la ligne de commande</param>
        /// <param name="regexOptions">Combinaison d'opérations de bits des valeurs d'énumération qui fournissent des options pour la correspondance</param>
        /// <param name="callback">L'exécution de la ligne de commande</param>
        public void AddCommand(string pattern, RegexOptions regexOptions, Action<CommandTools> callback) {
            AddPatternAndCallback(pattern, regexOptions, callback);
        }

        /// <summary>
        /// L'ajout de l'interprétation d'une ligne de commande et de son exécution
        /// </summary>
        /// <param name="pattern">L'expression réguliére d'interprétation de la ligne de commande</param>
        /// <param name="regexOptions">Combinaison d'opérations de bits des valeurs d'énumération qui fournissent des options pour la correspondance</param>
        /// <param name="callback">L'exécution de la ligne de commande</param>
        public void AddCommand(string pattern, RegexOptions regexOptions, Func<CommandTools, Task> callback) {
            AddPatternAndCallback(pattern, regexOptions, callback);
        }

        private void AddPatternAndCallback(string pattern, RegexOptions regexOptions, object callback) {
            ThrowIfPatternAndCallbackInvalid(pattern, callback);
            _commandPatternCallbacks.Add(new CommandPattern {
                                             Pattern = pattern,
                                             RegexOptions = regexOptions
                                         }, callback);
        }

        private void ThrowIfPatternAndCallbackInvalid(string pattern, object command) {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (_commandPatternCallbacks.Keys.Any(k => k.Pattern == pattern))
                throw new ArgumentException("Pattern déjà présent", nameof(pattern));
        }

        /// <summary>
        /// Lance de la console en attente des lignes de commandes
        /// </summary>
        public async Task RunAsync() {
            ConfigureOptions();
            ShowHeader();
            string command = WaitNextCommand();
            while (!IsExitCommand(command)) {
                await RunCommandAsync(command);
                ShowLineBreakBetweenCommands();
                command = WaitNextCommand();
            }
        }

        private void ConfigureOptions() {
            _options = CreateDefaultOptions();
            _configureOptions?.Invoke(_options);
        }

        private static ConsoleOptions CreateDefaultOptions() {
            return new ConsoleOptions {
                Prompt = ">",
                LineBreakBetweenCommands = false,
                HeaderText = "",
                TrimCommand = true
            };
        }

        private void ShowHeader() {
            if (!string.IsNullOrEmpty(_options.HeaderText))
                Console.WriteLine(_options.HeaderText);
        }

        private string WaitNextCommand() {
            string command;
            do {
                PromptCommand();
                command = Console.ReadLine() ?? "";
                command = _options.TrimCommand ? command.Trim() : command;
            } while (string.IsNullOrEmpty(command));
            return command;
        }

        private void PromptCommand() {
            Console.Write(_options.Prompt);
        }

        private static bool IsExitCommand(string command) {
            return command.Equals("exit", StringComparison.OrdinalIgnoreCase);
        }

        private async Task RunCommandAsync(string command) {
            try {
                foreach (var (commandPattern, callback) in _commandPatternCallbacks.Select(x => (x.Key, x.Value))) {
                    Match match = Regex.Match(command, commandPattern.Pattern, commandPattern.RegexOptions);
                    if (match.Success) {
                        CommandTools tools = CreateTools(command, match);
                        if (callback is Func<CommandTools, Task> funcAync)
                            await funcAync(tools);
                        else if (callback is Action<CommandTools> action)
                            action(tools);
                        return;
                    }
                }
                throw new Exception("Commande non trouvée");
            }
            catch (Exception ex) {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        private CommandTools CreateTools(string command, Match commandMatch) {
            return new CommandTools {
                Command = command,
                CommandMatch = commandMatch,
                CommandArgs = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                AzureDevOps = new AzureDevOpsTools(_azureDevOpsServers)
            };
        }

        private void ShowLineBreakBetweenCommands() {
            if (_options.LineBreakBetweenCommands)
                Console.WriteLine();
        }
    }
}
