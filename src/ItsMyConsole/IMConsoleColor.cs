using System;

namespace ItsMyConsole
{
    /// <summary>
    /// Définit des couleurs spécifiques pour le flux de sortie standard de la console
    /// </summary>
    public static class IMConsoleColor
    {
        /// <summary>
        /// Couleur pour une réprésentation de type "mutée"
        /// </summary>
        public static ConsoleColor Muted => ConsoleColor.DarkGray;

        /// <summary>
        /// Couleur pour une réprésentation de type "avertissement"
        /// </summary>
        public static ConsoleColor Warning => ConsoleColor.Yellow;

        /// <summary>
        /// Couleur pour une réprésentation de type "danger"
        /// </summary>
        public static ConsoleColor Danger => ConsoleColor.DarkRed;

        /// <summary>
        /// Couleur pour une réprésentation de type "succès"
        /// </summary>
        public static ConsoleColor Success => ConsoleColor.DarkGreen;

        /// <summary>
        /// Couleur pour une réprésentation de type "information"
        /// </summary>
        public static ConsoleColor Info => ConsoleColor.Cyan;
    }
}
