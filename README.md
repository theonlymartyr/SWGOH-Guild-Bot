# SWGOH Guild Tools

This bot started out as a hobby project to evaluate rosters to determine whether or not a player could complete a specific legendary event.
With the release of The Journey Guide in game, this function is no longer needed. It has since evolved into a 
DSR clone written from the ground up for THE SENATE alliance officers to use, specifically for TW compares with some additional officer and individual uses.

1. This bot uses the [SWGOH.Help API](https://api.swgoh.help/), you will need to sign up to get an api username and password to put in the config file

2. I also used the [C# wrapper](https://github.com/SdtBarbarossa/SWGOH-Help-Api-C-Sharp) Created By: [SdtBarbarossa](https://github.com/SdtBarbarossa)

3. For Discord Platform I used [This Example](https://github.com/DSharpPlus/Example-Bots/tree/master/DSPlus.Examples.CSharp.Ex03) by [Emzi0767](https://github.com/Emzi0767) to get started.

4. You will need to create a Discord app and get a token to put in the config.json to run this as well: [Here](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token) is a good walkthrough on that

Current commands include:
  --;;tw - allows you to compare guilds for a tw matchup. An ignore list can be configured before running to allow for players that didn't sign up
  --;;gac - allows you to compare your roster to another player for your GAC matchup
  --;;go - gives the basic breakdown of GP in your guild for TB spreadsheets
  --;;gs - gives a player by player breakdown of your guild.

Future plans:
  --guild caching to speed up TW command when the API is under heavy load
  --a recruit evaluator to evaluate potential recruits against guild requirements
