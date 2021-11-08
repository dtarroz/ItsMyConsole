echo off
cls

rem Sauvegarde du répertoire racine de lancement pour revenir dessus en fin de batch puis on se place dans le répertoire du script
set cnp_racine=%cd%
cd %~d0%~p0

nuget pack "..\src\ItsMyConsole\ItsMyConsole.csproj" -Build -Properties Configuration=Release


rem Retour au répertoire racine de lancement
cd %cnp_racine%