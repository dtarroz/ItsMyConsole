namespace ItsMyConsole
{
    /// <summary>
    /// Options de configuration de la console
    /// </summary>
    public class ConsoleOptions
    {
        /// <summary>
        /// Texte du prompt qui est affiché à gauche de la ligne de commande en attente de saisie
        /// Par défaut : ">"
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Indicateur de présence de saut de ligne entre les lignes de commande
        /// Par défaut : false
        /// </summary>
        public bool LineBreakBetweenCommands { get; set; }

        /// <summary>
        /// Texte de l'entete de la console affiche en premier avant l'attente de la premiére commande
        /// Par défaut : ""
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// Indicateur pour effectuer un trim en début et en fin de la ligne de commande avant son exécution
        /// Par défaut : true
        /// </summary>
        public bool TrimCommand { get; set; }
    }
}
