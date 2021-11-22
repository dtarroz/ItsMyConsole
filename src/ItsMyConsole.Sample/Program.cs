using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ItsMyConsole.Sample
{
    class Program
    {
        static async Task Main() {
            ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

            ccli.Configure(options => {
                options.Prompt = ">> ";
                options.LineBreakBetweenCommands = true;
                options.HeaderText = "###################\n#  Hello, world!  #\n###################\n";
                options.TrimCommand = true;
            });

            ccli.AddCommand("^g (.+)$", RegexOptions.IgnoreCase, tools => {
                string search = tools.CommandMatch.Groups[1].Value.Replace(" ", "+");
                Process.Start($"https://www.google.fr/search?q={search}");
            });

            await ccli.RunAsync();
        }
    }
}
