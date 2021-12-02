namespace ItsMyConsole
{
    /// <summary>
    /// Options de configuration de la console
    /// </summary>
    public class ConsoleOptions
    {
        /// <summary>
        /// Texte du prompt qui est affiché à gauche de la ligne de commande en attente de saisie.<br/>
        /// (Par défaut : ">")
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Indicateur de présence de saut de ligne entre les lignes de commande.<br/>
        /// (Par défaut : false)
        /// </summary>
        public bool LineBreakBetweenCommands { get; set; }

        /// <summary>
        /// Texte de l'entête de la console qui s'affiche en premier avant l'attente de la première commande.<br/>
        /// (Par défaut : "")
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// Indicateur pour effectuer un trim en début et en fin de la ligne de commande avant son exécution.<br/>
        /// (Par défaut : true)
        /// </summary>
        public bool TrimCommand { get; set; }
    }
}
