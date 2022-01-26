using ItsMyConsole;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyExampleConsole
{
    class Program
    {
        // HttpClient is intended to be instantiated once per application, rather than per-use.
        static readonly HttpClient _httpClient = new HttpClient();

        static async Task Main() {
            ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

            // Console configuration
            ccli.Configure(options => {
                options.Prompt = ">> ";
                options.LineBreakBetweenCommands = true;
                options.HeaderText = "###################\n#  Hello, world!  #\n###################\n";
            });

            // Star Wars API (SWAPI) find person command implementation [Only results from page 1]
            // Example : sw sky
            ccli.AddCommand("^sw (.+)$", RegexOptions.IgnoreCase, async tools => {
                string search = tools.CommandMatch.Groups[1].Value;
                HttpResponseMessage response = await _httpClient.GetAsync($"https://swapi.dev/api/people?search={search}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                dynamic responseJson = JsonConvert.DeserializeObject(responseBody);
                foreach (dynamic people in responseJson.results)
                    Console.WriteLine(people.name);
            });

            await ccli.RunAsync();
        }
    }
}
