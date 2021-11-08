using System.Collections.Generic;
using System.Threading.Tasks;

namespace ItsMyConsole
{
    public class AzureDevOps
    {
        private readonly List<AzureDevOpsServer> _azureDevOpsServers;

        internal AzureDevOps(List<AzureDevOpsServer> azureDevOpsServers) {
            _azureDevOpsServers = azureDevOpsServers;
        }

        public async Task<object> GetWorkItemAsync(int workItemId) {
            await Task.Delay(1000);
            return null;
        }
    }
}
