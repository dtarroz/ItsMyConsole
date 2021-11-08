using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ItsMyConsole.Sample
{
    class Program
    {
        static void Main() {
            ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

            ccli.Configure(options => {
                options.Prompt = ">> ";
                options.LineBreakBetweenCommands = true;
                options.HeaderText = "###################\n#  Hello, world!  #\n###################\n";
                options.TrimCommand = true;
            });

            ccli.AddCommand("^g (.+)$", RegexOptions.IgnoreCase, action => {
                string search = action.CommandMatch.Groups[1].Value.Replace(" ", "+");
                Process.Start($"https://www.google.fr/search?q={search}");
            });

            /*ccli.AddCommand("^wi [0-9]*$", async action => {
                int workItemId = Convert.ToInt32(action.CommandArgs[1]);
                object workitem = await action.AzureDevOps.GetWorkItemAsync(workItemId);
                Console.WriteLine($"WI {workItemId} - <TITLE>");
            });*/

            ccli.Run();
        }
    }
}
