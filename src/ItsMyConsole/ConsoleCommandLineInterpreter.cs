using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ItsMyConsole
{
    public class ConsoleCommandLineInterpreter
    {
        private ConsoleOptions _options;
        private Action<ConsoleOptions> _configureOptions;
        private readonly Dictionary<ConsolePattern, Action<ConsoleAction>> _actionCommands;
        private readonly List<AzureDevOpsServer> _azureDevOpsServers;

        public ConsoleCommandLineInterpreter() {
            _actionCommands = new Dictionary<ConsolePattern, Action<ConsoleAction>>();
            _azureDevOpsServers = new List<AzureDevOpsServer>();
        }

        public void Configure(Action<ConsoleOptions> configureOptions) {
            _configureOptions = configureOptions;
        }

        public void AddAzureDevOpsServer(AzureDevOpsServer azureDevOpsServer) {
            if (_azureDevOpsServers.Any(a => a.Name == azureDevOpsServer.Name))
                throw new ArgumentException("Nom déjà présent", nameof(azureDevOpsServer.Name));
            if (string.IsNullOrWhiteSpace(azureDevOpsServer.Url))
                throw new ArgumentException("Url obligatoire", nameof(azureDevOpsServer.Url));
            if (string.IsNullOrWhiteSpace(azureDevOpsServer.PersonalAccessToken))
                throw new ArgumentException("Token personnel obligatoire", nameof(azureDevOpsServer.PersonalAccessToken));
            _azureDevOpsServers.Add(azureDevOpsServer);
        }

        public void AddCommand(string pattern, Action<ConsoleAction> command) {
            ThrowIfPatternAndActionInvalid(pattern, command);
            _actionCommands.Add(new ConsolePattern {
                                    Pattern = pattern,
                                }, command);
        }

        public void AddCommand(string pattern, RegexOptions regexOptions, Action<ConsoleAction> command) {
            ThrowIfPatternAndActionInvalid(pattern, command);
            _actionCommands.Add(new ConsolePattern {
                                    Pattern = pattern,
                                    RegexOptions = regexOptions
                                }, command);
        }

        private void ThrowIfPatternAndActionInvalid(string pattern, object command) {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (_actionCommands.Keys.Any(k => k.Pattern == pattern))
                throw new ArgumentException("Pattern déjà présent", nameof(pattern));
        }

        public void Run() {
            ConfigureOptions();
            ShowHeader();
            string command = WaitNextCommand();
            while (!IsExitCommand(command)) {
                RunCommand(command);
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

        private void RunCommand(string command) {
            try {
                foreach (var actionCommand in _actionCommands) {
                    Match match = Regex.Match(command, actionCommand.Key.Pattern, actionCommand.Key.RegexOptions);
                    if (match.Success) {
                        actionCommand.Value.Invoke(new ConsoleAction {
                                                       Command = command,
                                                       CommandMatch = match,
                                                       CommandArgs =
                                                           command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                                                       AzureDevOps = new AzureDevOps(_azureDevOpsServers)
                                                   });
                        return;
                    }
                }
                throw new Exception("Commande non trouvée");
            }
            catch (Exception ex) {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        private void ShowLineBreakBetweenCommands() {
            if (_options.LineBreakBetweenCommands)
                Console.WriteLine();
        }
    }
}
