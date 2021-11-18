using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
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
        public async Task<List<TeamSettingsIteration>> GetCurrentTeamIterationsAsync(
            string azureDevOpsName, string project, string team = null) {
            using (WorkHttpClient workHttpClient = GetWorkHttpClient(azureDevOpsName))
                return await workHttpClient.GetTeamIterationsAsync(new TeamContext(project, team), "Current");
        }

        private WorkHttpClient GetWorkHttpClient(string azureDevOpsName) {
            AzureDevOpsServer server = GetAzureDevOpsServer(azureDevOpsName);
            VssBasicCredential credentials = new VssBasicCredential("", server.PersonalAccessToken);
            return new WorkHttpClient(new Uri(server.Url), credentials);
        }

        /// <summary>
        /// Création d'un WorkItem
        /// </summary>
        /// <param name="azureDevOpsName">Le nom du serveur Azure Dev Ops qui a été configuré</param>
        /// <param name="workItemFields">Les champs du WorkItem</param>
        /// <returns>Le WorkItem crée</returns>
        public async Task<WorkItem> CreateWorkItemAsync(string azureDevOpsName, WorkItemFields workItemFields) {
            if (workItemFields == null)
                throw new ArgumentNullException(nameof(workItemFields));
            if (string.IsNullOrEmpty(workItemFields.TeamProject))
                throw new ArgumentException("L'équipe est obligatoire", nameof(workItemFields.TeamProject));
            if (string.IsNullOrEmpty(workItemFields.WorkItemType))
                throw new ArgumentException("Le type est obligatoire", nameof(workItemFields.WorkItemType));
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient(azureDevOpsName))
                return await workItemTrackingHttpClient.CreateWorkItemAsync(CreateJsonPatchDocument(workItemFields),
                                                                            workItemFields.TeamProject,
                                                                            workItemFields.WorkItemType);
        }

        private JsonPatchDocument CreateJsonPatchDocument(WorkItemFields workItemFields) {
            JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
            AddInJsonPatchDocument(Operation.Replace, "/fields/System.AreaPath", workItemFields.AreaPath, ref jsonPatchDocument);
            AddInJsonPatchDocument(Operation.Replace, "/fields/System.TeamProject", workItemFields.TeamProject,
                                   ref jsonPatchDocument);
            AddInJsonPatchDocument(Operation.Replace, "/fields/System.IterationPath", workItemFields.IterationPath,
                                   ref jsonPatchDocument);
            AddInJsonPatchDocument(Operation.Replace, "/fields/System.Title", workItemFields.Title, ref jsonPatchDocument);
            AddInJsonPatchDocument(Operation.Replace, "/fields/System.State", workItemFields.State, ref jsonPatchDocument);
            AddInJsonPatchDocument(Operation.Replace, "/fields/System.WorkItemType", workItemFields.WorkItemType,
                                   ref jsonPatchDocument);
            AddInJsonPatchDocument(Operation.Replace, "/fields/System.AssignedTo", workItemFields.AssignedTo,
                                   ref jsonPatchDocument);
            AddInJsonPatchDocument(Operation.Replace, "/fields/Microsoft.VSTS.Common.Activity", workItemFields.Activity,
                                   ref jsonPatchDocument);
            return jsonPatchDocument;
        }

        private void AddInJsonPatchDocument(Operation operation, string field, object value,
                                            ref JsonPatchDocument jsonPatchDocument) {
            if (value != null) {
                jsonPatchDocument.Add(new JsonPatchOperation {
                                          Operation = operation,
                                          Path = field,
                                          Value = value
                                      });
            }
        }

        /// <summary>
        /// Mise à jour d'un WorkItem
        /// </summary>
        /// <param name="azureDevOpsName">Le nom du serveur Azure Dev Ops qui a été configuré</param>
        /// <param name="workItemId">L'identifiant du WorkItem</param>
        /// <param name="workItemFields">Les champs du WorkItem à modifier</param>
        public async Task UpdateWorkItemAsync(string azureDevOpsName, int workItemId, WorkItemFields workItemFields) {
            if (workItemFields == null)
                throw new ArgumentNullException(nameof(workItemFields));
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient(azureDevOpsName))
                await workItemTrackingHttpClient.UpdateWorkItemAsync(CreateJsonPatchDocument(workItemFields), workItemId);
        }

        /// <summary>
        /// Ajoute plusieurs relations de WorkItems à un WorkItem
        /// </summary>
        /// <param name="azureDevOpsName">Le nom du serveur Azure Dev Ops qui a été configuré</param>
        /// <param name="workItemId">L'identifiant du WorkItem qui va recevoir la relation</param>
        /// <param name="workItemToAdd">le WorkItem à ajouter</param>
        /// <param name="linkTypes">Le type de lien (par exemple : System.LinkTypes.Hierarchy-Forward)</param>
        public async Task AddWorkItemRelationsAsync(string azureDevOpsName, int workItemId, WorkItem workItemToAdd,
                                                    string linkTypes) {
            await AddWorkItemRelationsAsync(azureDevOpsName, workItemId, new List<WorkItem> { workItemToAdd }, linkTypes);
        }

        /// <summary>
        /// Ajoute plusieurs relations de WorkItems à un WorkItem
        /// </summary>
        /// <param name="azureDevOpsName">Le nom du serveur Azure Dev Ops qui a été configuré</param>
        /// <param name="workItemId">L'identifiant du WorkItem qui va recevoir la relation</param>
        /// <param name="workItemsToAdd">Les WorkItems à ajouter</param>
        /// <param name="linkTypes">Le type de lien (par exemple : System.LinkTypes.Hierarchy-Forward)</param>
        public async Task AddWorkItemRelationsAsync(string azureDevOpsName, int workItemId, List<WorkItem> workItemsToAdd,
                                                    string linkTypes) {
            if (workItemsToAdd == null)
                throw new ArgumentNullException(nameof(workItemsToAdd));
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = GetWorkItemTrackingHttpClient(azureDevOpsName)) {
                JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
                foreach (WorkItem workItem in workItemsToAdd) {
                    AddInJsonPatchDocument(Operation.Add, "/relations/-", new {
                                               rel = linkTypes,
                                               url = workItem.Url,
                                           }, ref jsonPatchDocument);
                }
                await workItemTrackingHttpClient.UpdateWorkItemAsync(jsonPatchDocument, workItemId);
            }
        }
    }
}
