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

        public void Configure(Action<ConsoleOptions> configureOptions) {
            _configureOptions = configureOptions;
        }

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

        public void AddCommand(string pattern, Action<CommandOutils> callback) {
            AddPatternAndCallback(pattern, RegexOptions.None, callback);
        }

        public void AddCommand(string pattern, Func<CommandOutils, Task> callback) {
            AddPatternAndCallback(pattern, RegexOptions.None, callback);
        }

        public void AddCommand(string pattern, RegexOptions regexOptions, Action<CommandOutils> callback) {
            AddPatternAndCallback(pattern, regexOptions, callback);
        }

        public void AddCommand(string pattern, RegexOptions regexOptions, Func<CommandOutils, Task> callback) {
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
                        CommandOutils outils = CreateOutils(command, match);
                        if (callback is Func<CommandOutils, Task> funcAync)
                            await funcAync(outils);
                        else if (callback is Action<CommandOutils> action)
                            action(outils);
                        return;
                    }
                }
                throw new Exception("Commande non trouvée");
            }
            catch (Exception ex) {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        private CommandOutils CreateOutils(string command, Match commandMatch) {
            return new CommandOutils {
                Command = command,
                CommandMatch = commandMatch,
                CommandArgs = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                AzureDevOps = new AzureDevOps(_azureDevOpsServers)
            };
        }

        private void ShowLineBreakBetweenCommands() {
            if (_options.LineBreakBetweenCommands)
                Console.WriteLine();
        }
    }
}
