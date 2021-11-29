![Logo](docs/logo.png)

# ItsMyConsole

Framework pour application Console .Net pour la construction d'interpréteur de ligne de commande interne.

## Sommaire
- [Pourquoi faire ?](#pourquoi-faire-)
- [Getting Started](#getting-started)
- [Configurer les options](#configurer-les-options)
- [Configurer les interprétations de commande](#configurer-les-interprétations-de-commande)
- [Commande "exit"](#commande-exit)
- [Outils](#outils)
- [Comment créer ses propres Outils ?](#comment-créer-ses-propres-outils-)

## Pourquoi faire ?
Vous allez pouvoir créer une application Console .Net qui attend la saisie d'une ligne de commande pour lancer vos propres actions spécifiques. 

Le Framework ```ItsMyConsole``` met en place pour vous :

- L'attente de la saisie d'une ligne de commande avec un prompt personnalisé
- La mise en attente d'une nouvelle commande après chaque exécution
- L'ajout d'une implémentation de l'action à effectuer selon un pattern *(expression régulière)* pour la commande saisie
- La commande *"exit"* qui ferme automatiquement l'application
- La configuration du comportement générale de la Console
    - Texte de l'entête
    - Texte du prompt avant chaque attente de commande
    - Trim *(ou non)* de la ligne de commande (suppression automatique des espaces en début ou fin de la saisie)
    - Saut de ligne *(ou non)* entre chaque attente d'une nouvelle saisie de commande

## Getting Started
1. Créer un projet **"Application Console .Net"** avec le nom *"MyExampleConsole"*
2. Ajouter ```ItsMyConsole``` au projet depuis le gestionnaire de package NuGet
3. Pour cet exemple, ajouter en plus ```Newtonsoft.Json``` au projet depuis le gestionnaire de package NuGet
4. Modifier la méthode **"Main"** dans le fichier **"Program.cs"** par le code suivant :
```cs
using ItsMyConsole;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyExampleConsole
{
    class Program
    {
        // HttpClient is intended to be instantiated once per application, rather than per-use.
        static readonly HttpClient _httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

            // Console configuration 
            ccli.Configure(options =>
            {
                options.Prompt = ">> ";
                options.LineBreakBetweenCommands = true;
                options.HeaderText = "###################\n#  Hello, world!  #\n###################\n";
            });

            // Star Wars API (SWAPI) find person command implementation [Only results from page 1] 
            // Example : sw sky
            ccli.AddCommand("^sw (.+)$", RegexOptions.IgnoreCase, async tools =>
            {
                string search = tools.CommandMatch.Groups[1].Value;
                HttpResponseMessage response = await _httpClient.GetAsync($"https://swapi.dev/api/people?search={search}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                dynamic responseJson = JsonConvert.DeserializeObject(responseBody);
                foreach (dynamic people in responseJson.results)
                    Console.WriteLine(people.name);
            });

            await ccli.RunAsync();
        }
    }
}
```

Voici le résultat attendu lors de l'utilisation de la Console :

![MyExampleProject](docs/MyExampleProject.png) 

Dans cet exemple de code, on a configuré avec ```Configure``` le prompt d’attente des commandes ```options.Prompt```, la présence d'un saut de ligne entre les saisies ```options.LineBreakBetweenCommands``` et l’en-tête affichée au lancement ```options.HeaderText```. 

Puis avec ```AddCommand```, on a ajouté un pattern d’interprétation des lignes de commande ```^sw (.*)$``` *(commence par **"sw"** puis on capture le reste de la commande)* qui est insensible à la casse ```RegexOptions.IgnoreCase```.

Lors de l'exécution de la Console, si on saisie une commande qui commence par **"sw"** avec du contenu à la suite, il lancera l'implémentation de l'action associée. Dans cet exemple, il récupère le reste de la commande en utilisant ```tools.CommandMatch``` depuis les outils disponibles *(résultat du Match de l'expression régulière)* pour faire une recherche sur les personnages de l'univers Star Wars depuis une API dédiée *(seulement les premiers résultats)*. Avec les informations récupérées, il affiche les noms des personnages dans la Console.

Maintenant que l'on a configuré la Console et l'implémention de l'action associée au pattern ```^sw (.*)$```, l'utilisation de ```RunAsync``` lance la mise en attente d'une saisie de commande par l'utilisateur.

## Configurer les options
*coming soon*

## Configurer les interprétations de commande
*coming soon*

## Commande "exit"
*coming soon*

## Outils
*coming soon*

## Comment créer ses propres Outils ?
*coming soon*
