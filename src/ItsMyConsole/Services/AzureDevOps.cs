using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItsMyConsole
{
    public class AzureDevOps
    {
        private readonly List<AzureDevOpsServer> _azureDevOpsServers;

        static AzureDevOps() {
            Environment.SetEnvironmentVariable("VSS_ALLOW_UNSAFE_BASICAUTH", "true");
        }

        internal AzureDevOps(List<AzureDevOpsServer> azureDevOpsServers) {
            _azureDevOpsServers = azureDevOpsServers;
        }

        public async Task<WorkItem> GetWorkItemAsync(string azureDevOpsName, int workItemId, WorkItemExpand? expand = null) {
            WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient(azureDevOpsName);
            return await workItemTrackingHttpClient.GetWorkItemAsync(workItemId, expand: expand);
        }

        private WorkItemTrackingHttpClient GetWorkItemTrackingHttpClient(string azureDevOpsName) {
            AzureDevOpsServer server = GetAzureDevOpsServer(azureDevOpsName);
            VssBasicCredential credentials = new VssBasicCredential("", server.PersonalAccessToken);
            return new WorkItemTrackingHttpClient(new Uri(server.Url), credentials);
        }

        private AzureDevOpsServer GetAzureDevOpsServer(string azureDevOpsName) {
            AzureDevOpsServer server = _azureDevOpsServers.FirstOrDefault(a => a.Name == azureDevOpsName);
            if (server == null)
                throw new Exception($"Le serveur Azure Dev Ops '{azureDevOpsName}' n'a pas été trouvé");
            return server;
        }
    }
}
