using System.Text.RegularExpressions;

namespace ItsMyConsole
{
    public class ConsoleAction
    {
        public string Command { get; set; }
        public Match CommandMatch { get; set; }
        public string[] CommandArgs { get; set; }
        public AzureDevOps AzureDevOps { get; set; }

        internal ConsoleAction() { }
    }
}
