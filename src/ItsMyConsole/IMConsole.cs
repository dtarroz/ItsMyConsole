using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ItsMyConsole
{
    /// <summary>
    /// Fournit des méthodes utilitaires supplémentaires pour simplifier et enrichir les interactions avec la console
    /// </summary>
    public static class IMConsole
    {
        private static readonly object ThisLock = new object();

        /// <summary>
        /// Écrit la représentation textuelle de l'objet spécifié dans le flux de sortie standard de la console
        /// </summary>
        /// <param name="value">La valeur à écrire</param>
        /// <param name="foregroundColor">La couleur du texte (par défaut : couleur par défaut de console)</param>
        /// <param name="backgroundColor">La couleur d'arriére plan (par défaut : couleur par défaut de console)</param>
        public static void Write(object value, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null) {
            lock (ThisLock) {
                if (foregroundColor != null)
                    Console.ForegroundColor = foregroundColor.Value;
                if (backgroundColor != null)
                    Console.BackgroundColor = backgroundColor.Value;
                Console.Write(value);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Écrit la représentation textuelle de l'objet spécifié, suivie du terminateur de ligne actuel,
        /// dans le flux de sortie standard de la console
        /// </summary>
        /// <param name="value">La valeur à écrire</param>
        /// <param name="foregroundColor">La couleur du texte (par défaut : couleur par défaut de console)</param>
        /// <param name="backgroundColor">La couleur d'arriére plan (par défaut : couleur par défaut de console)</param>
        public static void WriteLine(object value, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null) {
            lock (ThisLock) {
                if (foregroundColor != null)
                    Console.ForegroundColor = foregroundColor.Value;
                if (backgroundColor != null)
                    Console.BackgroundColor = backgroundColor.Value;
                Console.WriteLine(value);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Écrit un ou plusieurs terminateur de ligne actuel dans le flux de sortie standard de la console
        /// </summary>
        /// <param name="count">Le nombre de terminateur de ligne (par défaut : 1)</param>
        public static void LineBreak(int count = 1) {
            for (int i = 0; i < count; i++)
                Console.WriteLine();
        }

        /// <summary>
        /// Minimise la fenêtre de la console après un délai. Cette méthode fonctionne uniquement sous Windows.
        /// </summary>
        /// <param name="delay">Le délai en secondes (par défaut : aucun délai)</param>
        public static async Task MinimizeAsync(int delay = 0) {
            if (delay > 0)
                await Task.Delay(delay * 1000);
            Minimize();
        }

        /// <summary>
        /// Minimise la fenêtre de la console. Cette méthode fonctionne uniquement sous Windows.
        /// </summary>
        public static void Minimize() {
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            if (handle != IntPtr.Zero)
                ShowWindow(handle, 6 /*SW_MINIMIZE*/);
        }

        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow([In] IntPtr hWnd, [In] int nCmdShow);

        /// <summary>
        /// Demande à l'utilisateur de confirmer une action en saisissant "oui" ou "non".
        /// </summary>
        /// <param name="message">Le message de confirmation</param>
        /// <param name="foregroundColor">La couleur du texte (par défaut : couleur par défaut de console)</param>
        /// <param name="backgroundColor">La couleur d'arriére plan (par défaut : couleur par défaut de console)</param>
        /// <returns>true si "oui", sinon false</returns>
        public static bool Confirm(string message, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null) {
            Write($"{message} [oui] : ", foregroundColor, backgroundColor);
            string response = Console.ReadLine()?.ToLower() ?? "";
            return new[] { "oui", "o", "yes", "y", "" }.Contains(response);
        }
    }
}
