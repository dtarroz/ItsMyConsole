![Logo](docs/logo.png)

# ItsMyConsole

Framework pour application Console .Net pour la construction d'interpréteur de ligne de commande interne.

## Sommaire
- [Pourquoi faire ?](#pourquoi-faire-)
- [Getting Started](#getting-started)
- [Configurer les options](#configurer-les-options)
- [Ajouter des interprétations de commande](#ajouter-des-interprétations-de-commande)
- [Les outils](#les-outils)
- [Commande "exit"](#commande-exit)
- [Méthodes d'extension de la console](#méthodes-dextension-de-la-console)
- [Ajouter d'autres outils](#ajouter-dautres-outils)
- [Comment créer ses propres outils ?](#comment-créer-ses-propres-outils-)
- [Lancement de la console](#lancement-de-la-console)

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
    - L'ajout automatique du symbole de début et de fin pour le pattern de la commande
    - Les options par défaut pour la correspondance au pattern de commande *(exemple : ignorer la casse)*
    - La couleur du texte *(le texte de l'entête, le prompt et la commande saisie)*

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

        static async Task Main()
        {
            ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

            // Console configuration 
            ccli.Configure(options => {
                options.Prompt = ">> ";
                options.LineBreakBetweenCommands = true;
                options.HeaderText = "###################\n#  Hello, world!  #\n###################\n";
            });

            // Star Wars API (SWAPI) find person command implementation [Only results from page 1] 
            // Example : sw sky
            ccli.AddCommand("^sw (.+)$", RegexOptions.IgnoreCase, async tools => {
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

Dans cet exemple de code on a configuré avec ```Configure```, le prompt d’attente des commandes ```options.Prompt```, la présence d'un saut de ligne entre les saisies ```options.LineBreakBetweenCommands``` et l’en-tête affichée au lancement ```options.HeaderText```. 

Puis avec ```AddCommand```, on a ajouté un pattern d’interprétation des lignes de commande ```^sw (.*)$``` *(commence par **"sw"** puis on capture le reste de la commande)* qui est insensible à la casse ```RegexOptions.IgnoreCase```.

Lors de l'exécution de la Console, si on saisie une commande qui commence par **"sw"** avec du contenu à la suite, il lancera l'implémentation de l'action associée. Dans cet exemple, il récupère le reste de la commande en utilisant ```tools.CommandMatch``` depuis les outils disponibles *(résultat du Match de l'expression régulière)* pour faire une recherche sur les personnages de l'univers Star Wars depuis une API dédiée *(seulement les premiers résultats)*. Avec les informations récupérées, il affiche les noms des personnages dans la Console.

Maintenant que l'on a configuré la Console et l'implémentation de l'action associée au pattern ```^sw (.*)$```, l'utilisation de ```RunAsync``` lance la mise en attente d'une saisie de commande par l'utilisateur.

## Configurer les options

Vous pouvez configurer les options de la Console en utilisant ```Configure```.

| Nom de l'option | Description | Valeur par défaut |
| :-------------- | :---------- | :---------------: |
| Prompt | Texte du prompt qui est affiché à gauche de la ligne de commande en attente de saisie | ">" |
| LineBreakBetweenCommands | Indicateur de présence de saut de ligne entre les lignes de commande | false |
| HeaderText | Texte de l'entête de la console qui s'affiche en premier avant l'attente de la première commande | "" |
| TrimCommand | Indicateur pour effectuer un trim en début et en fin de la ligne de commande avant son exécution | true |
| DefaultCommandRegexOptions | Options par défaut des expressions régulières pour l'interprétation d'une commande | RegexOptions.None |
| AddStartAndEndCommandPatternAuto | Indicateur pour ajouter automatiquement le symbole de début *"^"* et de fin *"$"* sur l'expression régulière pour l'interprétation d'une commande, lorsqu'ils ne sont pas présent | false |
| HeaderTextColor | Couleur du texte de l'entête de la console | Couleur par défaut |
| PromptColor | Couleur du texte du prompt | Couleur par défaut |
| CommandColor | Couleur du texte saisie de la commande | Couleur par défaut |

```cs
ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

// Console configuration 
ccli.Configure(options =>
{
    options.Prompt = ">> ";
    options.LineBreakBetweenCommands = true;
    options.HeaderText = "###################\n#  Hello, world!  #\n###################\n";
});
```

## Ajouter des interprétations de commande

Vous pouvez ajouter des interprétations de commande en utilisant ```AddCommand```.

| Nom de l'argument | Description |
| :---------------- | :---------- |
| pattern | L'expression régulière d'interprétation de la ligne de commande. [Aide Mémoire](https://docs.microsoft.com/fr-fr/dotnet/standard/base-types/regular-expression-language-quick-reference) |
| regexOptions | *(facultatif)*<br/><br/>Combinaison d'opérations de bits des valeurs d'énumération qui fournissent des options pour la correspondance de l'expression régulière. *(exemple ```RegexOptions.IgnoreCase```)* [Lien vers la documentation](https://docs.microsoft.com/fr-fr/dotnet/api/system.text.regularexpressions.regexoptions?view=netstandard-2.0) |
| callback | L'action (ou la fonction async) pour l'exécution de la commande associé au pattern |

Si vous avez configuré ```AddStartAndEndCommandPatternAuto``` à ```true```, il ajoute automatiquement le symbole de début *"^"* et de fin *"$"* sur l'expression régulière pour l'interprétation d'une commande, lorsqu'ils ne sont pas présent.

Si vous ne spécifiez pas de valeur pour ```regexOptions```, il prendra la valeur configuré sur ```DefaultCommandRegexOptions``` (par défaut : ```RegexOptions.None```).

```cs
ConsoleCommandLineInterpreter ccli = new ConsoleCommandLineInterpreter();

// Simple action with ignore case pattern
ccli.AddCommand("^sw (.+)$", RegexOptions.IgnoreCase, tools =>
{
    // Insert your code here
});

// Simple function async without regex option
ccli.AddCommand("^delay$", async tools =>
{
    await Task.Delay(1000);  // await example
    // Insert your code here
});
```

## Les outils

Dans l'implémentation de l'action, vous avez accès à des outils *(nommé ```tools``` dans l'exemple ci-dessus)*. Les outils par défaut sont une aide pour l'interprétation de la ligne de commande saisie mais Il est possible d'ajouter d'autres **"outils"** avec NuGet ([exemples](#ajouter-dautres-outils)). Vous avez aussi la possibilité [d'en créer vous même](#comment-créer-ses-propres-outils-).

| Nom de l'outil | Description |
| :------------- | :---------- |
| Command | La ligne de commande saisie par l'utilisateur |
| CommandMatch | Le résultat du Match de l'expression régulière de la ligne de commande |
| CommandArgs | La liste des arguments de la ligne de commande. Le caractère ```" "``` est le séparateur. |

## Commande "exit"
Pour fermer l'application Console, vous avez par défaut l'interprétation de la commande ```exit``` *(insensible à la casse)* inclus dans le Framework.

## Méthodes d'extension de la console

### Ecrire en couleur dans la console

Vous pouvez écrire en couleur dans la console en utilisant ```IMConsole.Write``` ou ```IMConsole.WriteLine```.

| Nom de l'argument | Description |
| :---------------- | :---------- |
| value | Écrit la représentation textuelle de l'objet spécifié dans le flux de sortie standard de la console |
| foregroundColor | *(facultatif)* La couleur du texte (par défaut : couleur par défaut de console) |
| backgroundColor | *(facultatif)* La couleur d'arriére plan (par défaut : couleur par défaut de console) |

```cs
IMConsole.Write("Message en jaune, ", ConsoleColor.Yellow);
IMConsole.WriteLine("message en rouge", ConsoleColor.Red);

IMConsole.Write("Message en vert avec un fond cyan", ConsoleColor.DarkGreen, ConsoleColor.Cyan);
```

Vous avez accès à une liste de couleur spécifiques 

| Nom de la couleur | Description |
| :---------------- | :---------- |
| IMConsoleColor.Muted | Couleur pour une réprésentation de type "mutée" |
| IMConsoleColor.Warning | Couleur pour une réprésentation de type "avertissement" |
| IMConsoleColor.Danger | Couleur pour une réprésentation de type "danger" |
| IMConsoleColor.Success | Couleur pour une réprésentation de type "succès" |
| IMConsoleColor.Info | Couleur pour une réprésentation de type "information" |

```cs
IMConsole.WriteLine("Message d'erreur", IMConsoleColor.Danger);
```

### Ecrire un ou plusieurs saut de lignes

Vous pouvez écrire des saut de lignes dans la console en utilisant ```IMConsole.LineBreak```.

| Nom de l'argument | Description |
| :---------------- | :---------- |
| count | *(facultatif)* Le nombre de terminateur de ligne (par défaut : 1) |

```cs
IMConsole.LineBreak();

IMConsole.LineBreak(2);
```

### Minimiser la fenêtre de la console

Vous pouvez minimiser la fenêtre de la console en utilisant ```IMConsole.Minimize```. Cette méthode fonctionne uniquement sous Windows.
Vous pouvez ajouter un délai en secondes en utilisant ```IMConsole.MinimizeAsync```.

| Nom de l'argument | Description |
| :---------------- | :---------- |
| delay | *(facultatif)* Le délai en secondes (par défaut : aucun délai) |

```cs
IMConsole.Confirm("Voulez-vous continuer ?");
```

### Attendre une confirmation de l'utilisateur

Vous pouvez attendre une confirmation de l'utilisateur en utilisant ```IMConsole.Confirm```.

| Nom de l'argument | Description |
| :---------------- | :---------- |
| message | Le message de confirmation |
| foregroundColor | *(facultatif)* La couleur du texte (par défaut : couleur par défaut de console) |
| backgroundColor | *(facultatif)* La couleur d'arriére plan (par défaut : couleur par défaut de console) |

## Ajouter d'autres outils
Vous pouvez ajouter d'autres outils pour étendre et simplifier vos implémentations d'actions de vos commandes :

- [Azure Dev Ops](https://github.com/dtarroz/ItsMyConsole.Tools.AzureDevOps) : Création et modification des WorkItems sur Azure Dev Ops 
- [Cache Global](https://github.com/dtarroz/ItsMyConsole.Tools.GlobalCache) : Cache accessible par toutes les actions des commandes
- [Presse papier Windows](https://github.com/dtarroz/ItsMyConsole.Tools.Windows.Clipboard) : Lecture et écriture dans le presse papier Windows

## Comment créer ses propres Outils ?
*coming soon*

## Lancement de la console

Vous pouvez lancer la console et les interprétations des commandes en utilisant ```RunAsync```.

```cs
await ccli.RunAsync();
```

Vous avez aussi la possibilité de passer une commande directement pour exécuter seulement celle-ci.

```cs
await ccli.RunAsync("sw sky");
```
