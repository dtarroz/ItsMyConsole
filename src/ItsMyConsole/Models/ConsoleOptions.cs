using System;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Options par défaut des expressions réguliéres pour l'interprétation d'une commande<br/>
        /// (Par défaut : RegexOptions.None)
        /// </summary>
        public RegexOptions DefaultCommandRegexOptions { get; set; }

        /// <summary>
        /// Indicateur pour ajouter automatiquement le symbole de début "^" et de fin "$" sur l'expression réguliére pour
        /// l'interprétation d'une commande, lorsqu'ils ne sont pas présent<br/>
        /// (Par défaut : false)
        /// </summary>
        public bool AddStartAndEndCommandPatternAuto { get; set; }

        /// <summary>
        /// Couleur du texte de l'entête de la console.<br/>
        /// (Par défaut : Couleur standard de la console)
        /// </summary>
        public ConsoleColor HeaderTextColor { get; set; }

        /// <summary>
        /// Couleur du texte du prompt.<br/>
        /// (Par défaut : Couleur standard de la console)
        /// </summary>
        public ConsoleColor PromptColor { get; set; }

        /// <summary>
        /// Couleur du texte saisie de la commande.<br/>
        /// (Par défaut : Couleur standard de la console)
        /// </summary>
        public ConsoleColor CommandColor { get; set; }
    }
}
