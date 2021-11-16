using System.Text.RegularExpressions;

namespace ItsMyConsole
{
    public class CommandTools
    {
        /// <summary>
        /// La ligne de commande saisie par l'utilisateur
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Le résultat du Match de l'expression réguliére de la ligne de commande
        /// </summary>
        public Match CommandMatch { get; set; }

        /// <summary>
        /// La liste des arguments de la ligne de commande
        /// Le caractère "espace" est le séparateur
        /// </summary>
        public string[] CommandArgs { get; set; }

        /// <summary>
        /// L'accès aux serveurs Azure Dev Ops configurés
        /// </summary>
        public AzureDevOpsTools AzureDevOps { get; set; }

        internal CommandTools() { }
    }
}
