# SWGOH-Legendary-Hero-Prerequisite-Checker

This bot is a side project that is intended to give a player insight into whether or not they are able to get a "Legendary" or "Hero's Journey Character in SWGOH.

This bot uses the [SWGOH.Help API](https://api.swgoh.help/), you will need to sign up to get an api username and password to put in the config file (

I also used the [C# wrapper](https://github.com/SdtBarbarossa/SWGOH-Help-Api-C-Sharp) Created By: [SdtBarbarossa](https://github.com/SdtBarbarossa)

For Discord Platform I used [This Example](https://github.com/DSharpPlus/Example-Bots/tree/master/DSPlus.Examples.CSharp.Ex03) by [Emzi0767](https://github.com/Emzi0767) to get started.

It will pull a player's roster and check against star requirements for the event. If the player has the character, it will show character details instead.

Possible future plans to add the ability to give gear level recommendations on an individual use. This could be tricky.

Future plans to add the ability to see how the entire guild sits in relation to the event requirements, showing:
    -If guildmates have the given character, show star level and power level
    -If guildmates do not have the character, display needed character stats (this may be difficult for characters like padme where there is a larger faction able to be used)
    -Possibly 
