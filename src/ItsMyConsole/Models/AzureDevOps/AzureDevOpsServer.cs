namespace ItsMyConsole
{
    /// <summary>
    /// Information sur un serveur Azure Dev Ops
    /// </summary>
    public class AzureDevOpsServer
    {
        /// <summary>
        /// Nom unique du serveur Azure Dev Ops qui sert de désignation lors de son utilisation
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// L'URL du serveur Azure Dev Ops
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Le token d'accès personnel au serveur Azure Dev Ops
        /// </summary>
        public string PersonalAccessToken { get; set; }
    }
}
