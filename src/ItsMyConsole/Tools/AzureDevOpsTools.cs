using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItsMyConsole
{
    /// <summary>
    /// Outils pour Azure Dev Ops
    /// </summary>
    public class AzureDevOpsTools
    {
        private readonly List<AzureDevOpsServer> _azureDevOpsServers;

        static AzureDevOpsTools() {
            Environment.SetEnvironmentVariable("VSS_ALLOW_UNSAFE_BASICAUTH", "true");
        }

        internal AzureDevOpsTools(List<AzureDevOpsServer> azureDevOpsServers) {
            _azureDevOpsServers = azureDevOpsServers;
        }

        /// <summary>
        /// Récupération des informations sur un WorkItem par son identifiant
        /// </summary>
        /// <param name="azureDevOpsName">Le nom du serveur Azure Dev Ops qui a été configuré</param>
        /// <param name="workItemId">L'identifiant du WorkItem</param>
        /// <param name="expand">Les paramètres d'expansion des attributs d'un WorkItem. Les valeurs possibles sont {None, Relations, Fields, Links, All}.</param>
        public async Task<WorkItem> GetWorkItemAsync(string azureDevOpsName, int workItemId, WorkItemExpand? expand = null) {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient(azureDevOpsName))
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

        /// <summary>
        /// Récupération des itérations courantes d'un projet
        /// </summary>
        /// <param name="azureDevOpsName">Le nom du serveur Azure Dev Ops qui a été configuré</param>
        /// <param name="project">Le nom du projet</param>
        /// <param name="team">Le nom de l'équipe</param>
        public async Task<List<TeamSettingsIteration>> GetCurrentTeamIterationsAsync(string azureDevOpsName, string project, string team = null)
        {
            using (WorkHttpClient workHttpClient = GetWorkHttpClient(azureDevOpsName))
                return await workHttpClient.GetTeamIterationsAsync(new TeamContext(project, team), "Current");
        }

        private WorkHttpClient GetWorkHttpClient(string azureDevOpsName) {
            AzureDevOpsServer server = GetAzureDevOpsServer(azureDevOpsName);
            VssBasicCredential credentials = new VssBasicCredential("", server.PersonalAccessToken);
            return new WorkHttpClient(new Uri(server.Url), credentials);
        }
    }
}
