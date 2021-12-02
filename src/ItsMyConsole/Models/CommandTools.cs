using System.Text.RegularExpressions;

namespace ItsMyConsole
{
    /// <summary>
    /// Outils accessible pendant l'éxécution de la ligne de commande
    /// </summary>
    public class CommandTools
    {
        /// <summary>
        /// La ligne de commande saisie par l'utilisateur
        /// </summary>
        public string Command { get; internal set; }

        /// <summary>
        /// Le résultat du Match de l'expression régulière de la ligne de commande
        /// </summary>
        public Match CommandMatch { get; internal set; }

        /// <summary>
        /// La liste des arguments de la ligne de commande.<br/>
        /// Le caractère " " est le séparateur.
        /// </summary>
        public string[] CommandArgs { get; internal set; }

        internal CommandTools() { }
    }
}
