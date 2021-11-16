using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
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


            ccli.AddAzureDevOpsServer(new AzureDevOpsServer
            {
                Name = "TEST",
                Url = "https://<SERVEUR>",
                PersonalAccessToken = "<TOKEN>"
            });


            ccli.AddCommand("^g (.+)$", RegexOptions.IgnoreCase, tools => {
                string search = tools.CommandMatch.Groups[1].Value.Replace(" ", "+");
                Process.Start($"https://www.google.fr/search?q={search}");
            });

            ccli.AddCommand("^wi [0-9]*$", async tools => {
                int workItemId = Convert.ToInt32(tools.CommandArgs[1]);
                WorkItem workItem = await tools.AzureDevOps.GetWorkItemAsync("TEST", workItemId);
                Console.WriteLine($"WI {workItemId} - {workItem.Fields["System.Title"]}");
            });

            await ccli.RunAsync();
        }
    }
}
