using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SWGOH_Prereqs;
using System.Data.SqlClient;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace SWGOH
{
    /// <summary>
    /// The Commands class. All comands for the bot are defined in here. 
    /// </summary>
    public class Commands
    {
        swgohHelpApiHelper helper;

        string[] GAToonList = new string[] { "GLREY", "SUPREMELEADERKYLOREN", "DARTHMALAK", "GENERALSKYWALKER", "DARTHREVAN", "BASTILASHANDARK", "GRIEVOUS", "PADMEAMIDALA", "JEDIKNIGHTREVAN", "DARTHTRAYA", "ENFYSNEST" };
        string[] TWToonList = new string[] { "GLREY", "SUPREMELEADERKYLOREN", "GENERALSKYWALKER", "DARTHMALAK", "PADMEAMIDALA", "JEDIKNIGHTREVAN", "DARTHREVAN", "BASTILASHANDARK", "GRIEVOUS", "DARTHTRAYA", "BOSSK", "GEONOSIANBROODALPHA", "ENFYSNEST", "CAPITALNEGOTIATOR", "CAPITALMALEVOLENCE", "HOUNDSTOOTH", "MILLENNIUMFALCON" };
        //Mod unit stat 5 is speed
        #region guildstats
        /// <summary>
        /// The guild stats command. This command pulls basic stats for the guild associated with the provided allycode.
        /// </summary>
        /// <returns>
        /// A single embed if guild is less than 25 members, 2 embeds if the member count is greater than 25 members. Each member contains their: GP, Toon GP, Ship GP, Total Relic toons, A list of toons at r7, Amount of toons r4+, Amount of toons g13 and Amount of toon g12
        /// </returns>
        /// <param name="allycode">Allycode to lookup</param>
        /// <param name="sort">What to sort the results by (optional, defaults to GP descending</param>
        [Command("gs"), Description("Gets guild stats"), Aliases("guildstats", "stats")]
        public async Task getPrereqs(CommandContext ctx, [Description("Ally Code to lookup")] string allycode = "", [Description("Sort Type. Can be gp or name, optionally followed by asc or desc. gp desc is default")] string sort = "")
        {
            uint ss;
            uint parsedAllyCode;
            if (allycode.Contains("-"))
            {
                parsedAllyCode = parseAllycode(allycode);
            }
            else
            {
                if (UInt32.TryParse(allycode, out ss))
                {
                    parsedAllyCode = parseAllycode(ss.ToString());
                }
                else
                {
                    sort = allycode;
                    parsedAllyCode = parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
                }
            }
            Console.WriteLine(parsedAllyCode);
            if (!(parsedAllyCode == 1))
            {
                DataHelper dh = new DataHelper(ctx);
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Stats",
                    Color = new DiscordColor(0xFF0000) // red
                };
                try
                {
                    string sortedBy = "GP DESCENDING";
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                    DiscordMessage m = await ctx.RespondAsync("processing request, standby...");

                    DateTime start = DateTime.Now;
                    // first retrieve the interactivity module from the client
                    var interactivity = ctx.Client.GetInteractivityModule();
                    String s = "";
                    //login to the API

                    login();
                    await m.ModifyAsync(m.Content + "\n\nFetching Guild......");
                    GuildParse.Guild guilds = dh.getGuild(new uint[] { parsedAllyCode }, helper);
                    if (guilds.guild.Length > 0)
                    {
                        swgohGGhelper help = new swgohGGhelper();
                        string info = help.getGGinfo(guilds.guild[0].Roster[0].AllyCode);
                        SwgohPlayer p1 = SwgohPlayer.FromJson(info);
                        int maxWidth = 75;
                        string header = $"[{guilds.guild[0].Name}](https://swgoh.gg/g/{p1.Data.GuildId + "/" + p1.Data.GuildName.Replace(" ", "-") + "/"}) Stats ";
                        int space = maxWidth - header.Length;
                        GuildParse.GuildMember guild = guilds.guild[0];
                        PlayerParse.Player players1 = dh.getInformation(m, guild, helper, 25);
                        //Build our output

                        var embed2 = new DiscordEmbedBuilder
                        {
                            Title = "Stats",
                            Color = new DiscordColor(0xFF0000) // red
                        };
                        await m.ModifyAsync(m.Content + "\n\nBuilding Stats......");
                        sortedBy = dh.buildPlayerStats(players1, embed, embed2, sort, guild.Id);
                        await m.ModifyAsync(m.Content + "\n\nBuilding Display......");
                        s += "**" + header + sortedBy + "**\n";
                        s += "======= Overview =======```\n";
                        s += dh.createLine("Members:", guild.Members.ToString());
                        s += dh.createLine("Total GP:", (guild.Gp / 1000000.0).ToString("0.##") + "M");
                        s += dh.createLine("Character GP:", (dh.buildCharGP(guild.Roster, "Char") / 1.0).ToString("###.##") + "M");
                        s += dh.createLine("Fleet GP:", (dh.buildCharGP(guild.Roster, "Fleet") / 1.0).ToString("###.##") + "M");
                        s += "```";
                        embed.Description = s;
                        await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                        if (ctx.Guild != null) { await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:", embed: embed); await ctx.RespondAsync("", embed: embed2); }
                        else { await ctx.RespondAsync(" Here is the information you requested:", embed: embed); await ctx.RespondAsync("", embed: embed2); }
                    }
                    DateTime end = DateTime.Now;
                    Console.WriteLine((end - start).TotalSeconds);
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));

                }
                catch (Exception e)
                {
                    dh.exceptionHandler(e, ctx, embed);
                }
            }
            else
            {
                await ctx.RespondAsync("User is not registered. Please register or provide an allycode.");
            }
        }
        #endregion
        #region guildOverview
        [Command("go"), Description("Gets guild overview"), Aliases("guildoverview")]
        public async Task getOverview(CommandContext ctx, [Description("Ally Code to lookup")] string allycode = "")
        {
            uint ss;
            uint parsedAllyCode;
            if (allycode.Contains("-"))
            {
                parsedAllyCode = parseAllycode(allycode);
            }
            else
            {
                if (UInt32.TryParse(allycode, out ss))
                {
                    parsedAllyCode = parseAllycode(ss.ToString());
                }
                else
                {
                    parsedAllyCode = parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
                }
            }
            if (!(parsedAllyCode == 1))
            {
                DataHelper dh = new DataHelper(ctx);
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Stats",
                    Color = new DiscordColor(0xFF0000) // red
                };
                try
                {
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                    DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
                    DateTime start = DateTime.Now;
                    // first retrieve the interactivity module from the client
                    var interactivity = ctx.Client.GetInteractivityModule();
                    String s = "";
                    //login to the API
                    login();

                    await m.ModifyAsync(m.Content + "\n\n Fetching guild");
                    GuildParse.Guild guilds = dh.getGuild(new uint[] { parsedAllyCode }, helper);
                    swgohGGhelper help = new swgohGGhelper();
                    string info = help.getGGinfo(guilds.guild[0].Roster[0].AllyCode);
                    SwgohPlayer p1 = SwgohPlayer.FromJson(info);

                    if (guilds.guild.Length > 0)
                    {
                        int maxWidth = 75;
                        string header = $"[{guilds.guild[0].Name}](https://swgoh.gg/g/{p1.Data.GuildId + "/" + p1.Data.GuildName.Replace(" ", "-") + "/"}) Overview ";
                        int space = maxWidth - header.Length;
                        await m.ModifyAsync(m.Content + "\n\n Building stats display");
                        var embed2 = new DiscordEmbedBuilder
                        {
                            Title = "Stats",
                            Color = new DiscordColor(0xFF0000) // red
                        };
                        s += "**" + header + "**\n";
                        s += "======= Overview =======```\n";
                        s += dh.createLine("Members:", guilds.guild[0].Members.ToString());
                        s += dh.createLine("Total GP:", (guilds.guild[0].Gp / 1000000.0).ToString("0.##") + "M");
                        s += dh.createLine("Character GP:", (dh.buildCharGP(guilds.guild[0].Roster, "Char") / 1.0).ToString("###.##") + "M");
                        s += dh.createLine("Fleet GP:", (dh.buildCharGP(guilds.guild[0].Roster, "Fleet") / 1.0).ToString("###.##") + "M");
                        s += "```";
                        embed.Description = s;
                        await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                        await m.DeleteAsync();
                        if (ctx.Guild != null) { await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:", embed: embed); }
                        else { await ctx.RespondAsync(" Here is the information you requested:", embed: embed); }
                    }
                    DateTime end = DateTime.Now;
                    Console.WriteLine((end - start).TotalSeconds);
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                }
                catch (Exception e)
                {
                    dh.exceptionHandler(e, ctx, embed);
                }
            }
            else
            {
                await ctx.RespondAsync("User is not registered. Please register or provide an allycode.");
            }
        }
        #endregion
        #region TW
        /// <summary>
        /// The territory war command. This command pulls information for the guilds specified by the given allycodes.
        /// </summary>
        /// <returns>
        /// A single embed if guild is less than 25 members, 2 embeds if the member count is greater than 25 members. Each member contains their: GP, Toon GP, Ship GP, Total Relic toons, A list of toons at r7, Amount of toons r4+, Amount of toons g13 and Amount of toon g12
        /// </returns>
        /// <param name="allycode1">Allycode</param>
        /// <param name="sort">What to sort the results by (optional, defaults to GP descending</param>
        [Command("tw"), Description("Compares 2 guilds for TW")]
        public async Task TWCompare(CommandContext ctx, [Description("Ally code of one guild")] string allycode1, [Description("Ally Code of other guild")] string allycode2 = "")
        {
            uint ss;
            uint parsedAllyCode1 = 1, parsedAllyCode2 = 1;
            if (allycode2.Equals(""))
            {
                parsedAllyCode2 = parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
                parsedAllyCode1 = checkAllycode(ctx, allycode1);
            }
            else
            {
                parsedAllyCode1 = checkAllycode(ctx, allycode1);
                parsedAllyCode2 = checkAllycode(ctx, allycode2);
            }
            if ((!(parsedAllyCode1 == 1) && !(parsedAllyCode2 == 1)))
            {
                DataHelper dh = new DataHelper(ctx);
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Stats",
                    Color = new DiscordColor(0xFF0000) // red
                };
                try
                {
                    getTWCompare(ctx, parsedAllyCode1, parsedAllyCode2);
                }
                catch (Exception e)
                {
                    dh.exceptionHandler(e, ctx, embed);
                }
            }
            else
            {
                await ctx.RespondAsync("User is not registered. Please register or provide an allycode.");
            }
        }
        #endregion
        #region Registration
        [Command("reg"), Description("register allycode to discord handle"), Aliases("r")]
        public async Task registerMember(CommandContext ctx, [Description("Ally Code to register")] string allycode)
        {
            try
            {
                await registerUser(ctx, parseAllycode(allycode));
            }
            catch (Exception e) { await registerUser(ctx, parseAllycode(allycode)); }
        }
        [Command("unreg"), Description("register allycode to discord handle"), Aliases("ur")]
        public async Task unregisterMember(CommandContext ctx)
        {
            await unregisterUser(ctx, ctx.Member.Id.ToString());
        }
        [Command("checkusers"), Description("register allycode to discord handle"), Aliases("cu"), Hidden]
        public async Task checkMember(CommandContext ctx, [Description("Ally Code to check")] uint allycode)
        {
            var client = new MongoClient("mongodb+srv://the_only_martyr:swgohpassword@cluster0-9qh8n.mongodb.net/test?retryWrites=true&w=majority");
            var database = client.GetDatabase("BotUsers");
            var collection = database.GetCollection<BsonDocument>("Users");

            await ctx.RespondAsync(": Checking allycode: " + allycode);
        }
        #endregion
        [Command("invite"), Description("Invite the bot to your server"), Hidden]
        public async Task inviteBot(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "ASN-121 Help",
                Color = new DiscordColor(0xFF0000) // red
            };
            embed.Title = "Invite ASN-121 to your server.";
            embed.Description = "https://discordapp.com/api/oauth2/authorize?client_id=572518319991685120&permissions=68608&scope=bot";
            await ctx.RespondAsync("", embed: embed);
        }
        #region guild Char
        [Command("gc"), Description("Gets guild stats"), Aliases("guildchar")]
        public async Task guildChar(CommandContext ctx, [Description("Toon to lookup in the guild")] string toon, [Description("Ally Code to lookup")] string allycode = "")
        {

            uint sd;
            uint parsedAllyCode;
            if (allycode.Contains("-"))
            {
                parsedAllyCode = parseAllycode(allycode);
            }
            else
            {
                if (UInt32.TryParse(allycode, out sd))
                {
                    parsedAllyCode = parseAllycode(sd.ToString());
                }
                else
                {
                    parsedAllyCode = parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
                }
            }
            Console.WriteLine(parsedAllyCode);
            if (!(parsedAllyCode == 1))
            {
                //  CharacterStrings d = new CharacterStrings();
                String toonKey = "";
                DataHelper dh = new DataHelper(ctx);
                if (!toon.ToLower().Contains("list"))
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(0xFF0000) // red
                    };
                    try
                    {
                        /*      d.toonsList.TryGetValue(toon, out toonKey);
                              if (toonKey.Length > 0)
                              {*/
                        DateTime start = DateTime.Now;
                        // first retrieve the interactivity module from the client
                        var interactivity = ctx.Client.GetInteractivityModule();

                        String s = "";
                        //login to the API
                        login();
                        await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                        DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
                        int tries = 0;
                        GuildParse.Guild guilds = null;
                        while (tries < 5)
                        {
                            try
                            {
                                guilds = dh.getGuild(new uint[] { parsedAllyCode }, helper);
                                break;
                            }
                            catch { tries++; }
                        }
                        await m.ModifyAsync(m.Content + "\n\n....");
                        if (guilds != null)
                        {
                            if (guilds.guild.Length > 0)
                            {
                                swgohGGhelper help = new swgohGGhelper();
                                string info = help.getGGinfo(guilds.guild[0].Roster[0].AllyCode);
                                SwgohPlayer p1 = SwgohPlayer.FromJson(info);
                                int maxWidth = 75;
                                string header = $"[{guilds.guild[0].Name}](https://swgoh.gg/g/{p1.Data.GuildId + "/" + p1.Data.GuildName.Replace(" ", "-") + "/"}) {toon} Stats ";
                                int space = maxWidth - header.Length;
                                GuildParse.GuildMember guild = guilds.guild[0];
                                PlayerParse.Player players1 = dh.getInformation(m, guild, helper, 25);
                                //Build our output

                                s += "**" + header + "**\n";
                                embed.Description = s;
                                await m.ModifyAsync("Building output..");
                                dh.guildChar(players1, ctx, embed, readToons(ctx, toon));
                                await m.DeleteAsync();
                                await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:", embed: embed);
                                await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                                help = null;
                                p1 = null;
                                guild = null;
                                players1 = null;
                                await m.DeleteAsync();
                            }
                        }
                        else
                        {
                            await ctx.RespondAsync(ctx.User.Mention + " therewas an .");
                        }
                        /* }
                         else
                         {
                             await ctx.RespondAsync(ctx.User.Mention + " the toon name you entered was not found, please try again.");
                         }*/
                    }
                    catch (NullReferenceException ex)
                    {
                        dh.exceptionHandler(ex, ctx, embed);
                    }
                }
                else
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(0xFF0000) // red
                    };
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                    String ss = "";
                    embed.Title = "== Character List ==\n";
                    int z = 0;

                    /* foreach (string s in d.toonsList.Keys)
                     {
                         ss += s + "\n";
                         if (z % 30 == 0 && z != 0)
                         {
                             embed.AddField($"==", ss, true);
                             ss = "";
                         }
                         z++;
                     }
                     if (ss.Length > 0)
                         embed.AddField($"==", ss, true);
                         */
                    await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:", embed: embed);
                }
            }
            else
            {
                await ctx.RespondAsync("User is not registered. Please register or provide an allycode.");
            }
        }
        #endregion
        #region Help
        [Command("help"), Description("This Command List")]
        public async Task Help(CommandContext ctx, [RemainingText][Description("")] string user)
        {
            DataHelper dh = new DataHelper(ctx);
            string title = "", embeds = "", prefix = "";

            var embed = new DiscordEmbedBuilder
            {
                Title = "ASN-121 Help",
                Color = new DiscordColor(0xFF0000) // red
            };
            IReadOnlyDictionary<string, Command> commands = ctx.CommandsNext.RegisteredCommands;
            if (user == null)
            {
                DiscordMessage m = ctx.Message;
                prefix = m.Content.Replace(ctx.Command.Name, "");
                title = "```asciidoc\n= Command List =\n";
                embeds += title + $"Use {prefix}help commandName :: to get more information for each command\n\n";
                String comName = "";
                int i = 0;
                foreach (Command c in commands.Values)
                {
                    if (!comName.Equals(c.Name))
                    {
                        comName = c.Name;
                        i = 0;
                    }
                    else { i++; }

                    if (i < 1)
                    {
                        if (!c.IsHidden)
                        {
                            if (c.Name.Equals("tw") || c.Name.Equals("gs") || c.Name.Equals("reg") || c.Name.Equals("checkusers") || c.Name.Equals("help") || c.Name.Equals("gac") || c.Name.Equals("go") || c.Name.Equals("unreg"))
                            {
                                string aliases = "";
                                if (c.Aliases != null)
                                {
                                    if (c.Aliases.Count == 1) { foreach (string s in c.Aliases) { aliases += s; } }
                                    else
                                    {
                                        foreach (string s in c.Aliases) { aliases += s + ", "; }
                                        aliases = aliases.Substring(0, aliases.LastIndexOf(','));
                                    }
                                    embeds += dh.createLineDocs(prefix + c.Name, c.Description + " (alias: " + aliases + ")");
                                }
                                else { embeds += dh.createLineDocs(prefix + c.Name, c.Description); }
                            }
                        }
                    }
                }
                embeds += "```";
                embed.Description = embeds;
            }
            else
            {
                if (dh.checkCommand(user, commands))
                {
                    DiscordMessage m = ctx.Message;
                    prefix = m.Content.Substring(0, m.Content.IndexOf(ctx.Command.Name));
                    switch (user)
                    {
                        case "gs":
                        case "guildstats":
                        case "stats":
                            title = "```asciidoc\n= Guild Stats =\n";
                            embeds += title + "";
                            embeds += "Diplays data for every member of the guild of the provided allycode. \n\n" +
                                "Usage: gs {allycode} {sort} ::  allycode is required. Sort is optional, defaults to GP Descending.\n\n" +
                                "Sorts::  gp desc, gp asc, relics desc, relics asc, name desc and name asc\n\n" +
                                $"Example:: {prefix}gs 729778685 relics desc";
                            break;
                        case "tw":
                            title = "```asciidoc\n= TW Comparison =\n";
                            embeds += title + "";
                            embeds += "Displays a comparison of 2 guilds for TW. Both allycodes must be entered and represent 2 different guilds.\n\n" +
                                $"Example:: {prefix}tw 729778685 632295218";
                            break;
                        case "reg":
                        case "r":
                            title = "```asciidoc\n= Registration =\n";
                            embeds += title + "";
                            embeds += "Registers a user to the bot. Only one allycode can be registered per Discord user\n\n" +
                                $"Example:: {prefix}r 729778685 \n\n" +
                                $"Example:: {prefix}r 729778685 @the_only_martyr\n\n";
                            break;
                        case "unreg":
                        case "ur":
                            title = "```asciidoc\n= Registration Removal =\n";
                            embeds += title + "";
                            embeds += "Unregisters a user from the bot\n\n" +
                                $"Example:: {prefix}ur";
                            break;
                        case "checkusers":
                        case "cu":
                            title = "```asciidoc\n= Check Registration =\n";
                            embeds += title + "";
                            embeds += "Checks to see if a user is registered with the bot.\n\n" +
                                $"Example:: {prefix}cu 729778685";
                            break;
                        case "gac":
                            title = "```asciidoc\n= Grand Arena Comparison=\n";
                            embeds += title + "";
                            embeds += "Compares 2 players for GAC purposes.\n\n" +
                                $"Example:: {prefix}gac 729778685 733134572";
                            break;
                        case "go":
                            title = "```asciidoc\n= Guild Overview =\n";
                            embeds += title + "";
                            embeds += "A simple Overview of the guild.\n\n" +
                                $"Example:: {prefix}go 729778685";
                            break;
                    }
                    Console.WriteLine(prefix);
                    embeds += "```";
                    embed.Description = embeds;
                }
                else { embed.Description = "You must enter a valid command for me to assist you."; }
            }
            await ctx.RespondAsync("", embed: embed);
        }
        #endregion
        #region TW Compare Logic
        public async void getTWCompare(CommandContext ctx, uint allycode1, uint allycode2)
        {

            DateTime start = DateTime.Now;
            DataHelper dh = new DataHelper(ctx);
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
            DiscordMessage m = await ctx.RespondAsync("processing request, standby...");

            var interactivity = ctx.Client.GetInteractivityModule();
            String s = "", title = "", embeds = "";

            //login to the API
            login();
            //Retreive the 2 guilds to compare using an allycode for a member of each guild
            //If both allycodes are from the same guild, only one guild will return, make sure to check for that
            if (helper.loggedIn)
            {
                int tries = 0;
                GuildParse.Guild guilds = null;
                while (tries < 5)
                {
                    try
                    {
                        await m.ModifyAsync(m.Content + $"\n\n....attempting to retrieve guilds. Try {tries + 1}/5");
                        guilds = dh.getGuild(new uint[] { allycode1, allycode2 }, helper);
                        await m.ModifyAsync($"\n\n....Guilds successfully retrieved.\n\n");
                        break;
                    }
                    catch { tries++; }
                }
                if (guilds != null)
                {
                    if (guilds.guild.Length > 1)
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Title = "TW Comparison",
                            Color = new DiscordColor(0xFF0000) // red
                        };
                        int maxWidth = 75;

                        //Get guilds and build arrays of players
                        GuildParse.GuildMember guild1 = guilds.guild[0];
                        GuildParse.GuildMember guild2 = guilds.guild[1];

                        /*************
                         * Make sure we have all guild members for each guild
                         * Start with Guild 1
                         ************/
                        PlayerParse.Player players1, players2;
                        try
                        {
                            players1 = dh.getInformation(m, guild1, helper, 25);
                        }
                        catch
                        {
                            players1 = dh.getInformation(m, guild1, helper, 10);
                        }
                        /*************
                         * Make sure we have all guild members for each guild
                         * On to Guild 2
                         ************/
                        try
                        {
                            players2 = dh.getInformation(m, guild2, helper, 25);
                        }
                        catch
                        {
                            players2 = dh.getInformation(m, guild2, helper, 10);
                        }
                        if (players1 == null || players2 == null)
                        {
                            await ctx.RespondAsync(ctx.User.Mention + " There was an API error. Please try again.");
                            dh.logCommandInformation("Unsuccessful");
                        }
                        else
                        {
                            if (guild1.Members == players1.PlayerList.Length && guild2.Members == players2.PlayerList.Length)
                            {
                                string link1 = "", link2 = "";
                                for (int i = 0; i < players1.PlayerList.Length; i++)
                                {
                                    try
                                    {
                                        swgohGGhelper help = new swgohGGhelper();
                                        string info = help.getGGinfo((uint)players1.PlayerList[i].AllyCode);
                                        SwgohPlayer p1 = SwgohPlayer.FromJson(info);
                                        link1 = $"https://swgoh.gg/g/{p1.Data.GuildId + "/" + p1.Data.GuildName.Replace(" ", "-") + "/"}";
                                        break;
                                    }
                                    catch { }
                                }
                                for (int i = 0; i < players2.PlayerList.Length; i++)
                                {
                                    try
                                    {
                                        swgohGGhelper help = new swgohGGhelper();
                                        string info = help.getGGinfo((uint)players2.PlayerList[i].AllyCode);
                                        SwgohPlayer p2 = SwgohPlayer.FromJson(info);
                                        link2 = $"https://swgoh.gg/g/{p2.Data.GuildId + "/" + p2.Data.GuildName.Replace(" ", "-") + "/"}";
                                        break;
                                    }
                                    catch { }
                                }
                                string header = $"[{guilds.guild[0].Name}]({link1})  VS  [{guilds.guild[1].Name}]({link2})";
                                int space = maxWidth - header.Length;

                                ToonStats ts1 = new ToonStats();
                                ToonStats ts2 = new ToonStats();
                                TWGuild guild1st = dh.newGuildStats(players1, TWToonList);
                                TWGuild guild2nd = dh.newGuildStats(players2, TWToonList);
                                //Build our output
                                #region Header Output
                                s += "**" + header + "**\n";
                                s += "======= Overview =======```\n";
                                try
                                {
                                    s += dh.createLongLine("Members:", (guild1.Members - ((guild1st.ignoreList != null) ? guild1st.ignoreList.Count : 0)).ToString(), (guild2.Members - ((guild2nd.ignoreList != null) ? guild2nd.ignoreList.Count : 0)).ToString());
                                }
                                catch { s += dh.createLongLine("Members:", guild1.Members.ToString(), guild2.Members.ToString()); }
                                s += dh.createLongLine("Total GP:", (guild1.Gp / 1000000).ToString("0.##") + "M", (guild2.Gp / 1000000).ToString("0.##") + "M");
                                s += dh.createLongLine("Character GP:", dh.buildCharGP(guild1.Roster, "Char").ToString("0.##") + "M", dh.buildCharGP(guild2.Roster, "Char").ToString("0.##") + "M");
                                s += dh.createLongLine("Fleet GP:", dh.buildCharGP(guild1.Roster, "Fleet").ToString("0.##") + "M", dh.buildCharGP(guild2.Roster, "Fleet").ToString("0.##") + "M");
                                s += "```";
                                embed.Description = s;

                                #endregion
                                #region gear
                                title = "==Gear==\n";
                                embeds += "```CSS\n";
                                embeds += dh.createLongLine("G13:", guild1st.G13.ToString(), guild2nd.G13.ToString());
                                embeds += dh.createLongLine("G12+5:", guild1st.G125.ToString(), guild2nd.G125.ToString());
                                embeds += dh.createLongLine("G12+4:", guild1st.G124.ToString(), guild2nd.G124.ToString());
                                embeds += dh.createLongLine("G12+3:", guild1st.G123.ToString(), guild2nd.G123.ToString());
                                embeds += dh.createLongLine("G12+2:", guild1st.G122.ToString(), guild2nd.G122.ToString());
                                embeds += dh.createLongLine("G12+1:", guild1st.G121.ToString(), guild2nd.G121.ToString());
                                embeds += dh.createLongLine("G12:", guild1st.G12.ToString(), guild2nd.G12.ToString());
                                embeds += dh.createLongLine("G11:", guild1st.G11.ToString(), guild2nd.G11.ToString());
                                embeds += "```";
                                embed.AddField($"{title}", embeds, false);
                                #endregion
                                #region relics
                                title = "==Relics==";
                                embeds = "```CSS\n";
                                embeds += dh.createLongLine("Total Relics:", guild1st.TotalRelics.ToString(), guild2nd.TotalRelics.ToString());
                                embeds += dh.createLongLine("Relic 7:", guild1st.relics[7].ToString(), guild2nd.relics[7].ToString());
                                embeds += dh.createLongLine("Relic 6:", guild1st.relics[6].ToString(), guild2nd.relics[6].ToString());
                                embeds += dh.createLongLine("Relic 5:", guild1st.relics[5].ToString(), guild2nd.relics[5].ToString());
                                embeds += dh.createLongLine("Relic 4:", guild1st.relics[4].ToString(), guild2nd.relics[4].ToString());
                                embeds += dh.createLongLine("Relic 3:", guild1st.relics[3].ToString(), guild2nd.relics[3].ToString());
                                embeds += dh.createLongLine("Relic 2:", guild1st.relics[2].ToString(), guild2nd.relics[2].ToString());
                                embeds += dh.createLongLine("Relic 1:", guild1st.relics[1].ToString(), guild2nd.relics[1].ToString());
                                embeds += dh.createLongLine("Relic 0:", guild1st.relics[0].ToString(), guild2nd.relics[0].ToString());
                                embeds += "```";
                                embed.AddField($"{title}", embeds, false);
                                #endregion
                                #region mods
                                title = "==Mods==";
                                embeds = "```CSS\n";
                                embeds += dh.createLongLine("6 Dot Mods:", guild1st.sixStarMods.ToString(), guild2nd.sixStarMods.ToString());
                                embeds += dh.createLongLine("25+ Speed:", guild1st.speedMods[3].ToString(), guild2nd.speedMods[3].ToString());
                                embeds += dh.createLongLine("20+ Speed:", guild1st.speedMods[2].ToString(), guild2nd.speedMods[2].ToString());
                                embeds += dh.createLongLine("15+ Speed:", guild1st.speedMods[1].ToString(), guild2nd.speedMods[1].ToString());
                                embeds += dh.createLongLine("10+ Speed:", guild1st.speedMods[0].ToString(), guild2nd.speedMods[0].ToString());
                                embeds += dh.createLongLine("100+ Off:", guild1st.off100.ToString(), guild2nd.off100.ToString());
                                embeds += "```";
                                embed.AddField($"{title}", embeds, false);
                                #endregion
                                CharacterDefID d = new CharacterDefID();
                                foreach (string toonName in TWToonList)
                                {
                                    Toons p1, p2;
                                    //Look for the desired toon in the players list, if it exists, the corresponding GAToon will be a copy of it, otherwise it will be null
                                    p1 = guild1st.toonList.Find(x => x.id.Equals(toonName));
                                    if (p1 == null) { p1 = new Toons(toonName, 6); guild1st.toonList.Add(p1); }
                                    p2 = guild2nd.toonList.Find(x => x.id.Equals(toonName));
                                    if (p2 == null) { p2 = new Toons(toonName, 6); guild2nd.toonList.Add(p2); }

                                    //if a particular toon is not in the players list, create a new empty toon for comparison reasons
                                    title = $"={d.toons[toonName]}=";
                                    embeds = "```CSS\n";
                                    if (p1.ship)
                                    {
                                        embeds += dh.createLine("Total:", p1.Total.ToString(), p2.Total.ToString());
                                        embeds += dh.createLine("7*:", p1.stars[2].ToString(), p2.stars[2].ToString());
                                        embeds += dh.createLine("6*:", p1.stars[1].ToString(), p2.stars[1].ToString());
                                        embeds += dh.createLine("5*:", p1.stars[0].ToString(), p2.stars[0].ToString());
                                    }
                                    else
                                    {
                                        //{ "GLREY", "SUPREMELEADERKYLOREN", "DARTHMALAK", "GENERALSKYWALKER", "DARTHREVAN", "BASTILASHANDARK", "GRIEVOUS", "PADMEAMIDALA", "JEDIKNIGHTREVAN", "DARTHTRAYA", "HOUNDSTOOTH", "ENFYSNEST", "BOSSK", "GEONOSIANBROODALPHA", "CAPITALNEGOTIATOR", "CAPITALMALEVOLENCE", "MILLENNIUMFALCON" };
                                        embeds += dh.createLine("Total:", p1.Total.ToString(), p2.Total.ToString());
                                        embeds += dh.createLine("7*:", p1.stars[2].ToString(), p2.stars[2].ToString());
                                        embeds += dh.createLine("6*:", p1.stars[1].ToString(), p2.stars[1].ToString());
                                        embeds += dh.createLine("5*:", p1.stars[0].ToString(), p2.stars[0].ToString());
                                        embeds += dh.createLine("G13:", p1.gear[2].ToString(), p2.gear[2].ToString());
                                        embeds += dh.createLine("G12:", p1.gear[1].ToString(), p2.gear[1].ToString());
                                        embeds += dh.createLine("G11:", p1.gear[0].ToString(), p2.gear[0].ToString());
                                        embeds += dh.createLine("Relic 4+:", (p1.relics[4] + p1.relics[5] + p1.relics[6] + p1.relics[7]).ToString(), (p2.relics[4] + p2.relics[5] + p2.relics[6] + p2.relics[7]).ToString());
                                        embeds += dh.createLine("Relic 7:", p1.relics[7].ToString(), p2.relics[7].ToString());
                                        embeds += dh.createLine("GP 16K+:", p1.gp16.ToString(), p2.gp16.ToString());
                                        embeds += dh.createLine("GP 20K+:", p1.gp20.ToString(), p2.gp20.ToString());
                                        embeds += dh.createLine("1Z:", p1.numZetas[0].ToString(), p2.numZetas[0].ToString());
                                        if (!toonName.Equals("BASTILASHANDARK") && !toonName.Equals("ENFYSNEST"))
                                        {
                                            embeds += dh.createLine("2Z:", p1.numZetas[1].ToString(), p2.numZetas[1].ToString());
                                            if (toonName.Equals("GLREY") || toonName.Equals("SUPREMELEADERKYLOREN") || toonName.Equals("GENERALSKYWALKER") || toonName.Equals("DARTHREVAN") || toonName.Equals("JEDIKNIGHTREVAN"))
                                            {
                                                embeds += dh.createLine("3Z:", p1.numZetas[2].ToString(), p2.numZetas[2].ToString());
                                            }
                                            if (toonName.Equals("GLREY") || toonName.Equals("SUPREMELEADERKYLOREN") || toonName.Equals("GENERALSKYWALKER"))
                                            {
                                                embeds += dh.createLine("4Z:", p1.numZetas[3].ToString(), p2.numZetas[3].ToString());
                                            }
                                            if (toonName.Equals("GLREY") || toonName.Equals("SUPREMELEADERKYLOREN"))
                                            {
                                                embeds += dh.createLine("5Z:", p1.numZetas[4].ToString(), p2.numZetas[4].ToString());
                                                embeds += dh.createLine("6Z:", p1.numZetas[5].ToString(), p2.numZetas[5].ToString());
                                            }
                                        }
                                    }
                                    embeds += "```";
                                    embed.AddField($"{title}", embeds, true);
                                }
                                #region ignoring
                                try
                                {
                                    Console.WriteLine("Building Ignore embed");
                                    String list = "";
                                    if (guild1st.ignoreList != null)
                                    {
                                        foreach (String member in guild1st.ignoreList)
                                        {
                                            foreach (GuildParse.Roster r in guild1.Roster)
                                            {
                                                if (r.AllyCode.ToString().Equals(member))
                                                {
                                                    list += $"{r.Name} - {member}  \n\n";
                                                    Console.WriteLine($"{member} added to ignore embed");
                                                }
                                            }
                                        }
                                        title = "==Ignoring==";
                                        embeds = "```CSS\n";
                                        embeds += list;
                                        embeds += "```";
                                        embed.AddField($"{title}", embeds, false);
                                    }
                                    if (guild2nd.ignoreList != null)
                                    {
                                        foreach (String member in guild2nd.ignoreList)
                                        {
                                            foreach (GuildParse.Roster r in guild2.Roster)
                                            {
                                                if (r.AllyCode.ToString().Equals(member))
                                                {
                                                    list += $"{r.Name} - {member}  \n\n";
                                                    Console.WriteLine($"{member} added to ignore embed");
                                                }
                                            }
                                        }
                                        title = "==Ignoring==";
                                        embeds = "```CSS\n";
                                        embeds += list;
                                        embeds += "```";
                                        embed.AddField($"{title}", embeds, false);
                                    }
                                }
                                catch { Console.WriteLine("Building Ignore embed failed"); }
                                #endregion

                                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));

                                DateTime end = DateTime.Now;
                                Console.WriteLine((end - start).TotalSeconds);
                                dh.logCommandInformation("Successful");
                                await m.DeleteAsync();
                                await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:", embed: embed);
                            }
                            else
                            {
                                await ctx.RespondAsync(ctx.User.Mention + " it appears something has went wrong, please try again in a few minutes.");
                                dh.logCommandInformation("Unsuccessful");
                            }
                        }
                    }
                    else
                    {
                        await ctx.RespondAsync(ctx.User.Mention + " it appears you either supplied 2 allycodes from the same guild or did not supply 2 allycodes. Please try again.");
                        dh.logCommandInformation("Unsuccessful");
                    }
                }
                else
                {
                    await ctx.RespondAsync(ctx.User.Mention + " I had an issue retireving guild data. The API seems to be under heavy load. Please try again.");
                    dh.logCommandInformation("Unsuccessful");
                }
            }
            else
            {
                await m.DeleteAsync();
                await ctx.RespondAsync(ctx.User.Mention + " I had an issue retireving guild data. The API seems to be under heavy load. Please try again.");
            }
        }
        #endregion
        #region GA Compare Logic
        [Command("gac"), Description("Compare 2 players for GAC")]
        public async Task getGAC(CommandContext ctx, [Description("Ally Code to lookup")] string allycode1, string allycode2 = "")
        {
            uint parsedAllyCode1 = 1, parsedAllyCode2 = 1;
            if (allycode2.Equals(""))
            {
                parsedAllyCode2 = parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
                parsedAllyCode1 = checkAllycode(ctx, allycode1);
            }
            else
            {
                parsedAllyCode1 = checkAllycode(ctx, allycode1);
                parsedAllyCode2 = checkAllycode(ctx, allycode2);
            }
            if ((!(parsedAllyCode1 == 1) && !(parsedAllyCode2 == 1)))
            {
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
                var interactivity = ctx.Client.GetInteractivityModule();
                String s = "", title = "", embeds = "";
                DataHelper dh = new DataHelper(ctx);
                login();
                var embed = new DiscordEmbedBuilder
                {
                    Title = "GA Comparison",
                    Color = new DiscordColor(0xFF0000) // red
                };
                int maxWidth = 75;
                try
                {
                    GAPlayer[] players = dh.getGAStats(m, new uint[] { parsedAllyCode1, parsedAllyCode2 }, helper, GAToonList);

                    //Get players
                    string header = $"[{players[0].name}](https://swgoh.gg/p/{players[0].AllyCode}/gac-history/)  VS  [{players[1].name}](https://swgoh.gg/p/{players[1].AllyCode}/gac-history/)";
                    int space = maxWidth - header.Length;
                    //Build our output
                    #region Header Output
                    s += "**" + header + "**\n";
                    s += "======= Overview =======```\n";
                    s += dh.createHeaderLine("Total GP:", (players[0].totGP / 1000000.0).ToString("0.##") + "M", (players[1].totGP / 1000000.0).ToString("0.##") + "M");
                    s += dh.createHeaderLine("Character GP:", (players[0].toonGP / 1000000.0).ToString("0.##") + "M", (players[1].toonGP / 1000000.0).ToString("0.##") + "M");
                    s += dh.createHeaderLine("Fleet GP:", (players[0].shipGP / 1000000.0).ToString("0.##") + "M", (players[1].shipGP / 1000000.0).ToString("0.##") + "M");
                    s += "```";
                    #region GA
                    s += "==GA Stats==\n";
                    s += "```\n";
                    s += dh.createHeaderLine("Off Wins:", players[0].offWon.ToString(), players[1].offWon.ToString());
                    s += dh.createHeaderLine("Defends:", players[0].defend.ToString(), players[1].defend.ToString());
                    s += dh.createHeaderLine("Undersize:", players[0].under.ToString(), players[1].under.ToString());
                    s += dh.createHeaderLine("Full Clear:", players[0].fullClear.ToString(), players[1].fullClear.ToString());
                    s += dh.createHeaderLine("Banners:", (players[0].Banners / 1000.0).ToString("0.#K"), (players[1].Banners / 1000.0).ToString("0.#K").ToString());
                    s += "```";
                    embed.Description = s;
                    #endregion
                    #endregion
                    #region gear
                    title = "==Gear==\n";
                    embeds = "```CSS\n";
                    embeds += dh.createShortLine("G13:", players[0].G13.ToString(), players[1].G13.ToString());
                    embeds += dh.createShortLine("G12+5:", players[0].G125.ToString(), players[1].G125.ToString());
                    embeds += dh.createShortLine("G12+4:", players[0].G124.ToString(), players[1].G124.ToString());
                    embeds += dh.createShortLine("G12+3:", players[0].G123.ToString(), players[1].G123.ToString());
                    embeds += dh.createShortLine("G12+2:", players[0].G122.ToString(), players[1].G122.ToString());
                    embeds += dh.createShortLine("G12+1:", players[0].G121.ToString(), players[1].G121.ToString());
                    embeds += dh.createShortLine("G12:", players[0].G12.ToString(), players[1].G12.ToString());
                    embeds += dh.createShortLine("G11:", players[0].G11.ToString(), players[1].G11.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region relics
                    title = "==Relics==";
                    embeds = "```CSS\n";
                    embeds += dh.createShortLine("Total Relics:", players[0].totalRelics.ToString(), players[1].totalRelics.ToString());
                    embeds += dh.createShortLine("Relic 7:", players[0].relics[7].ToString(), players[1].relics[7].ToString());
                    embeds += dh.createShortLine("Relic 6:", players[0].relics[6].ToString(), players[1].relics[6].ToString());
                    embeds += dh.createShortLine("Relic 5:", players[0].relics[5].ToString(), players[1].relics[5].ToString());
                    embeds += dh.createShortLine("Relic 4:", players[0].relics[4].ToString(), players[1].relics[4].ToString());
                    embeds += dh.createShortLine("Relic 3:", players[0].relics[3].ToString(), players[1].relics[3].ToString());
                    embeds += dh.createShortLine("Relic 2:", players[0].relics[2].ToString(), players[1].relics[2].ToString());
                    embeds += dh.createShortLine("Relic 1:", players[0].relics[1].ToString(), players[1].relics[1].ToString());
                    embeds += dh.createShortLine("Relic 0:", players[0].relics[0].ToString(), players[1].relics[0].ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region mods
                    title = "==Mods==";
                    embeds = "```CSS\n";
                    embeds += dh.createShortLine("6 Dot Mods:", players[0].sixStarMods.ToString(), players[1].sixStarMods.ToString());
                    embeds += dh.createShortLine("10+ Speed:", players[0].speedMods[0].ToString(), players[1].speedMods[0].ToString());
                    embeds += dh.createShortLine("15+ Speed:", players[0].speedMods[1].ToString(), players[1].speedMods[1].ToString());
                    embeds += dh.createShortLine("20+ Speed:", players[0].speedMods[2].ToString(), players[1].speedMods[2].ToString());
                    embeds += dh.createShortLine("25+ Speed:", players[0].speedMods[3].ToString(), players[1].speedMods[3].ToString());
                    embeds += dh.createShortLine("100+ Off:", players[0].off100.ToString(), players[1].off100.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    CharacterDefID d = new CharacterDefID();
                    foreach (string toonName in GAToonList)
                    {
                        GAToon p1, p2;
                        //Look for the desired toon in the players list, if it exists, the corresponding GAToon will be a copy of it, otherwise it will be null
                        p1 = players[0].toonsOwned.Find(x => x.igName.Equals(toonName));
                        if (p1 == null) { p1 = new GAToon(toonName); players[0].toonsOwned.Add(p1); }
                        p2 = players[1].toonsOwned.Find(x => x.igName.Equals(toonName));
                        if (p2 == null) { p2 = new GAToon(toonName); players[1].toonsOwned.Add(p2); }

                        //if a particular toon is not in the players list, create a new empty toon for comparison reasons
                        title = $"={d.toons[toonName]}=";
                        embeds = "```CSS\n";
                        embeds += dh.createShortLine("GP:", p1.GP.ToString(), p2.GP.ToString());
                        embeds += dh.createShortLine("Stars:", p1.starLevel.ToString(), p2.starLevel.ToString());
                        embeds += dh.createShortLine("Gear:", p1.gearLevel.ToString() + (p1.gearEquipped > 0 ? $"+{p1.gearEquipped}" : ""), p2.gearLevel.ToString() + (p2.gearEquipped > 0 ? $"+{p2.gearEquipped}" : ""));
                        embeds += dh.createShortLine("Relics:", p1.relic.ToString(), p2.relic.ToString());
                        embeds += dh.createShortLine("Zetas:", p1.Zetas.ToString(), p2.Zetas.ToString());
                        embeds += dh.createShortLine("Speed:", p1.speed.ToString(), p2.speed.ToString());
                        embeds += dh.createShortLine("P Dam:", p1.physDam.ToString(), p2.physDam.ToString());
                        embeds += dh.createShortLine("S Dam:", p1.specDam.ToString(), p2.specDam.ToString());
                        embeds += "```";
                        embed.AddField($"{title}", embeds, true);
                    }
                    await m.DeleteAsync();
                    await ctx.RespondAsync(ctx.Member.Mention + " Here is your GA Comparison", embed: embed);
                }
                catch (Exception e)
                {
                    dh.exceptionHandler(e, ctx, embed);
                }
            }
            else
            {
                await ctx.RespondAsync("User is not registered. Please register or provide an allycode.");
            }
        }
        #endregion
        #region unregister Logic
        public async Task unregisterUser(CommandContext ctx, string userID)
        {
            JObject users;
            JArray user;
            String prefix;
            try
            {
                IReadOnlyDictionary<string, Command> commands = ctx.CommandsNext.RegisteredCommands;
                DiscordMessage m = ctx.Message;
                prefix = m.Content.Replace(ctx.Command.Name, "");
                bool found = false;
                //if file exists, parse it
                users = JObject.Parse(File.ReadAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\users.txt"));
                user = (JArray)users["users"];
                foreach (JObject obj in user.Children())
                {
                    if (obj.Property("id").Value.ToString().Equals(userID))
                    {
                        found = true;
                        user.Remove(obj);
                        break;
                    }
                }
                if (found)
                {
                    File.WriteAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\users.txt", users.ToString());
                    await ctx.RespondAsync($"<@{userID}> has unregistered. Register with a new allycode using ```;;r allycode```");
                }
                else
                {
                    await ctx.RespondAsync($"<@{userID}> was not found. Register with a new allycode using ```;;r allycode```");
                }
            }
            catch (Exception e)
            {
                await ctx.RespondAsync($"Something bad happened.\n{e.Message}\n{e.StackTrace}");
            }
        }
        #endregion
        #region register Logic
        public async Task registerUser(CommandContext ctx, uint allycode)
        {
            //Users.Users u = new Users.Users();
            JObject users;
            JArray user;
            DataHelper dh = new DataHelper(ctx);
            try
            {
                bool found = false; String foundCode = "";
                GuildParse.Guild g = dh.getGuild(new uint[] { allycode }, helper);
                //if file exists, parse it
                users = JObject.Parse(File.ReadAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\users.txt"));
                user = (JArray)users["users"];
                foreach (JObject obj in user.Children())
                {
                    if (obj.Property("id").Value.ToString().Equals(ctx.Member.Id.ToString()))
                    {
                        found = true;
                        foundCode = obj.Property("allycode").Value.ToString();
                    }
                }
                if (found)
                {
                    await ctx.RespondAsync($"<@{ctx.Member.Id.ToString()}> has already been registered to {foundCode}. If you want to change your allycode, please unregister and register again with the new allycode.");
                }
                else
                {
                    if (addUser(user, allycode, ctx.Member.Id.ToString(), g.guild[0].Id))
                    {
                        File.WriteAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\users.txt", users.ToString());
                        await ctx.RespondAsync("<@" + ctx.Member.Id.ToString() + "> has  been registered to " + allycode + ". If you want to change your allycode, please unregister and register again with the new allycode.");
                    }
                    else
                        await ctx.RespondAsync("<@" + ctx.Member.Id.ToString() + "> not registered to " + allycode + ". Check the allycode and try again.");
                }
            }
            catch (FileNotFoundException fnf)
            {
                //otherwise start a new one
                users = new JObject();
                user = new JArray();
                users["users"] = user;
                GuildParse.Guild g = dh.getGuild(new uint[] { allycode }, helper);
                if (addUser(user, allycode, ctx.Member.Id.ToString(), g.guild[0].Id))
                {
                    File.WriteAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\users.txt", users.ToString());
                    await ctx.RespondAsync("<@" + ctx.Member.Id.ToString() + "> has  been registered to " + allycode + ". If you want to change your allycode, please unregister and register again with the new allycode.");
                }
                else
                    await ctx.RespondAsync("<@" + ctx.Member.Id.ToString() + "> not registered to " + allycode + ". Check the allycode and try again.");
            }
        }
        public bool addUser(JArray arr, uint allycode, string userID, string guildID)
        {
            if (parseAllycode(allycode.ToString()) > 99999)
            {
                //Create new user
                JObject newUser = new JObject();
                newUser.Add("id", userID);
                newUser.Add("allycode", allycode);
                newUser.Add("guildID", guildID);
                //create default GA Toon list
                JArray GAToons = new JArray();
                string[] toons = { "GLREY", "SUPREMELEADERKYLOREN", "DARTHMALAK", "GENERALSKYWALKER", "DARTHREVAN", "BASTILASHANDARK", "GRIEVOUS", "PADMEAMIDALA", "JEDIKNIGHTREVAN", "DARTHTRAYA", "ENFYSNEST" };
                foreach (string s in toons)
                {
                    JObject toon = new JObject();
                    toon.Add("nameKey", s);
                    GAToons.Add(toon);
                }
                //Add toon list to object
                newUser.Add("GAToons", GAToons);
                //create default TW toon list
                JArray TWToons = new JArray();
                string[] twtoons = { "GLREY", "SUPREMELEADERKYLOREN", "GENERALSKYWALKER", "DARTHMALAK", "PADMEAMIDALA", "JEDIKNIGHTREVAN", "DARTHREVAN", "BASTILASHANDARK", "GRIEVOUS", "DARTHTRAYA", "BOSSK", "GEONOSIANBROODALPHA", "ENFYSNEST", "CAPITALNEGOTIATOR", "CAPITALMALEVOLENCE", "HOUNDSTOOTH", "MILLENNIUMFALCON" }; ;
                foreach (string s in twtoons)
                {
                    JObject toon = new JObject();
                    toon.Add("nameKey", s);
                    TWToons.Add(toon);
                }
                //Add toon list to object
                newUser.Add("TWToons", TWToons);
                //Add user to list
                arr.Add(newUser);
                return true;
            }
            return false;
        }
        public string checkRegistered(string userID)
        {
            JObject users;
            JArray user;
            bool found = false;
            Console.WriteLine("Checking users...");
            users = JObject.Parse(File.ReadAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\users.txt"));
            user = (JArray)users["users"];
            foreach (JObject obj in user.Children())
            {
                Console.WriteLine("searching...");
                if (obj.Property("id").Value.ToString().Equals(userID))
                {
                    found = true;
                    return obj.Property("allycode").Value.ToString();
                }
            }
            return "";
        }
        public string getGuildID(string userID)
        {
            JObject users;
            JArray user;
            bool found = false;
            Console.WriteLine("Checking users...");
            users = JObject.Parse(File.ReadAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\users.txt"));
            user = (JArray)users["users"];
            foreach (JObject obj in user.Children())
            {
                Console.WriteLine("searching...");
                if (obj.Property("id").Value.ToString().Equals(userID))
                {
                    found = true;
                    return obj.Property("guildID").Value.ToString();
                }
            }
            return "";
        }
        #endregion
        #region GA Toon Add/Remove
        [Command("gaa"), Description("Add a toon for GA Compare")]
        public async Task gaAdd(CommandContext ctx, [RemainingText]string toon)
        {
            await toonAdd(ctx, toon, "GA");
        }
        [Command("gar"), Description("Remove a toon for GA Compare")]
        public async Task gaRemove(CommandContext ctx, [RemainingText]string toon)
        {
            await toonRemove(ctx, toon, "GA");
        }
        #endregion
        #region TW Toon Add/Remove
        [Command("twa"), Description("Add a toon for GA Compare")]
        public async Task twAdd(CommandContext ctx, [RemainingText]string toon)
        {
            await toonAdd(ctx, toon, "TW");
        }
        [Command("twr"), Description("Remove a toon for GA Compare")]
        public async Task twRemove(CommandContext ctx, [RemainingText]string toon)
        {
            await toonRemove(ctx, toon, "TW");
        }
        #endregion
        #region Add/Remove Toon Logic
        public async Task toonList(CommandContext ctx, string userID, String mode)
        {
            JObject users;
            JArray user;
            DataHelper dh = new DataHelper(ctx);
            CharacterDefID d = new CharacterDefID();
            String toonName = "";
            try
            {
                bool added = false;
                //if file exists, parse it
                users = JObject.Parse(File.ReadAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\users.txt"));
                user = (JArray)users["users"];
                Console.WriteLine("Finding user");
                foreach (JObject obj in user.Children())
                {
                    if (obj.Property("id").Value.ToString().Equals(userID))
                    {
                        Console.WriteLine("User Found");
                        JArray toons = (JArray)obj[$"{mode}Toons"];
                        if (toons != null)
                        {
                            foreach (JObject o in toons)
                            {
                                toonName += d.toons[o["nameKey"].ToString()] + "\n";
                            }
                        }
                    }
                }
                Console.WriteLine(toonName);
            }
            catch (Exception e)
            {
                //    await ctx.RespondAsync($"{toon} not found");
            }
        }
        public async Task toonAdd(CommandContext ctx, string toon, String mode)
        {
            JObject users;
            JArray user;
            String userID = ctx.Member.Id.ToString();
            DataHelper dh = new DataHelper(ctx);
            CharacterStrings d = new CharacterStrings();
            String toonName = "";
            try
            {
                bool added = false;
                //if file exists, parse it
                users = JObject.Parse(File.ReadAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\users.txt"));
                user = (JArray)users["users"];
                toonName = d.toonsList[toon];
                Console.WriteLine("Finding user");
                foreach (JObject obj in user.Children())
                {
                    if (obj.Property("id").Value.ToString().Equals(userID))
                    {
                        Console.WriteLine("User Found");
                        JArray toons = (JArray)obj[$"{mode}Toons"];
                        if (toons == null)
                        {
                            Console.WriteLine("Toon list not found");
                            toons = new JArray();
                            obj.Add($"{mode}Toons", toons);
                        }
                        JObject j = (JObject)toons.SelectToken($"$.[?(@.nameKey=='{toonName}')]");
                        if ((j == null))
                        {
                            Console.WriteLine($"{toonName} not in list");
                            JObject newToon = new JObject();
                            newToon.Add("nameKey", toonName);
                            toons.Add(newToon);
                            added = true;
                        }
                    }
                }
                if (added)
                {
                    File.WriteAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\users.txt", users.ToString());
                    dh.logCommandInfo($"{ctx.Member.DisplayName} added {toonName} to their {mode} Toon List");
                    await ctx.RespondAsync($"{toon} has been added to your {mode} Toon List");
                }
                else
                    await ctx.RespondAsync($"{toon} was already in your {mode} Toon List");
            }

            catch (Exception e)
            {
                await ctx.RespondAsync($"{toon} not found");
            }
        }
        public async Task toonRemove(CommandContext ctx, string toon, String mode)
        {
            JObject users;
            JArray user;
            String userID = ctx.Member.Id.ToString();
            DataHelper dh = new DataHelper(ctx);
            CharacterStrings d = new CharacterStrings();
            String toonName = "";
            try
            {
                bool removed = false;
                //if file exists, parse it
                users = JObject.Parse(File.ReadAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\users.txt"));
                user = (JArray)users["users"];
                toonName = d.toonsList[toon];
                Console.WriteLine("Finding user");
                foreach (JObject obj in user.Children())
                {
                    if (obj.Property("id").Value.ToString().Equals(userID))
                    {
                        Console.WriteLine("User Found");
                        JArray toons = (JArray)obj[$"{mode}Toons"];
                        JObject j = (JObject)toons.SelectToken($"$.[?(@.nameKey=='{toonName}')]");
                        if (j != null)
                        {
                            Console.WriteLine("Toon in list");
                            toons.Remove(j);
                            removed = true;
                        }
                    }
                }
                if (removed)
                {
                    File.WriteAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\users.txt", users.ToString());
                    dh.logCommandInfo($"{ctx.Member.Nickname} removed {toonName} from their {mode} Toon List");
                    await ctx.RespondAsync($"{toon} has been removed from your {mode} Toon List");
                }
                else
                    await ctx.RespondAsync($"{toon} was not in your {mode} Toon List");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync($"{toon} not found");
            }
        }
        #endregion
        #region TW/GA Toon list
        [Command("gal"), Description("Invite the bot to your server"), Hidden]
        public async Task gal(CommandContext ctx)
        {
            await toonList(ctx, ctx.Member.Id.ToString(), "GA");
        }
        [Command("twl"), Description("Invite the bot to your server"), Hidden]
        public async Task twl(CommandContext ctx)
        {
            await toonList(ctx, ctx.Member.Id.ToString(), "TW");
        }
        #endregion
        [Command("readlist"), Description("Get the URL to a guilds swgoh.gg profile")]
        public async Task readlist(CommandContext ctx)
        {
            //readToons(ctx);
        }
        public string readDEFID(CommandContext ctx, string toon)
        {
            JObject toons;
            DataHelper dh = new DataHelper(ctx);
            //CharacterDefID d = new CharacterDefID();
            String toonName = "";
            try
            {
                //if file exists, parse it
                toons = JObject.Parse(File.ReadAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\defID.txt"));
                return toons.GetValue(toon, StringComparison.InvariantCultureIgnoreCase).ToString();

            }
            catch { }
            return "";
        }
        public string readToons(CommandContext ctx, string toon)
        {
            JObject toons;
            DataHelper dh = new DataHelper(ctx);
            //CharacterDefID d = new CharacterDefID();
            String toonName = "";
            try
            {
                //if file exists, parse it
                toons = JObject.Parse(File.ReadAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\toons.txt"));

                return toons.GetValue(toon, StringComparison.InvariantCultureIgnoreCase).ToString();

            }
            catch { }
            return "";
        }
        [Command("twi"), Description("Get the URL to a guilds swgoh.gg profile")]
        public async Task TWignore(CommandContext ctx, string allycode)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
            DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
            uint parsedAllyCode, sd;
            if (allycode.Contains("-"))
            {
                parsedAllyCode = parseAllycode(allycode);
            }
            else
            {
                if (UInt32.TryParse(allycode, out sd))
                {
                    parsedAllyCode = parseAllycode(sd.ToString());
                }
                else
                {
                    parsedAllyCode = parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
                }
            }
            login();
            // Console.WriteLine(parsedAllyCode);
            //  Console.WriteLine();
            uint userAlly = parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
            Console.WriteLine(userAlly);
            if (!(parsedAllyCode == 1))
            {
                //uint code = parseAllycode(allycode);
                JObject toons;
                DataHelper dh = new DataHelper(ctx);
                GuildParse.Guild g = dh.getGuild(new uint[] { userAlly }, helper);
                String guildID = getGuildID(ctx.Member.Id.ToString());
                //CharacterDefID d = new CharacterDefID();
                JObject users;
                JArray user;
                String userID = ctx.Member.Id.ToString();
                String toonName = "";
                try
                {
                    bool added = false, guildFound = false;
                    //if file exists, parse it
                    Console.WriteLine("Reading File");
                    users = JObject.Parse(File.ReadAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\Ignore.txt"));
                    user = (JArray)users["Guilds"];
                    Console.WriteLine("Finding Guild");
                    foreach (JObject obj in user.Children())
                    {
                        Console.WriteLine("Guild Search");
                        if (obj.Property("ID").Value.ToString().Equals(guildID))
                        {
                            Console.WriteLine("Guild Found");
                            guildFound = true;
                            JArray ignored = (JArray)obj[$"Ignored"];
                            if (ignored == null)
                            {
                                Console.WriteLine("Ignored List not found");
                                ignored = new JArray();
                                obj.Add($"Ignored", ignored);
                            }
                            else
                            {
                                Console.WriteLine("Ignored List found");
                            }
                            JObject j = (JObject)ignored.SelectToken($"$.[?(@.allycode=='{allycode}')]");
                            if (j == null)
                            {
                                Console.WriteLine($"{allycode} not in list");
                                JObject newToon = new JObject();
                                newToon.Add("allycode", allycode);
                                foreach (GuildParse.Roster r in g.guild[0].Roster)
                                {
                                    if (allycode.ToString().Equals(r.AllyCode.ToString())) { newToon.Add("name", r.Name); }
                                }
                                ignored.Add(newToon);
                                added = true;
                            }
                        }
                    }
                    if (!guildFound)
                    {
                        JObject guild = new JObject("ID", g.guild[0].Id.ToString());
                        user.Add(guild);
                        JArray ignored = new JArray();
                        guild.Add("Ignored", ignored);
                        ignored.Add(new JObject("allycode", allycode));
                        File.WriteAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\Ignore.txt", users.ToString());
                    }
                    else
                    {
                        if (added)
                        {
                            File.WriteAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\Ignore.txt", users.ToString());
                            dh.logCommandInfo($"{ctx.Member.DisplayName} added {allycode} to {g.guild[0].Id.ToString()} Ignore List");
                            await ctx.RespondAsync($"{allycode} has been added to your Ignore List");
                        }
                        else
                            await ctx.RespondAsync($"{allycode} was already in your Ignore List");
                    }
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                    await m.DeleteAsync();
                }
                catch (FileNotFoundException fnfe)
                {
                    users = new JObject();
                    user = new JArray();
                    JObject guild = new JObject();
                    guild.Add("ID", g.guild[0].Id.ToString());
                    JArray ignored = new JArray();
                    JObject ally = new JObject();
                    ally.Add("allycode", allycode);
                    ignored.Add(ally);
                    guild.Add("Ignored", ignored);
                    user.Add(guild);
                    users.Add("Guilds", user);
                    File.WriteAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\Ignore.txt", users.ToString());
                }
                catch (Exception e)
                {
                    await ctx.RespondAsync($"{allycode} not found");
                }

            }

        }
        [Command("twir"), Description("Get the URL to a guilds swgoh.gg profile")]
        public async Task TWremoveIgnore(CommandContext ctx, string allycode)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
            DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
            uint parsedAllyCode, sd;
            if (allycode.Contains("-"))
            {
                parsedAllyCode = parseAllycode(allycode);
            }
            else
            {
                if (UInt32.TryParse(allycode, out sd))
                {
                    parsedAllyCode = parseAllycode(sd.ToString());
                }
                else
                {
                    parsedAllyCode = parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
                }
            }
            login();
            uint userAlly = parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
            Console.WriteLine(userAlly);
            if (!(parsedAllyCode == 1))
            {
                //uint code = parseAllycode(allycode);
                JObject toons;
                DataHelper dh = new DataHelper(ctx);
                //GuildParse.Guild g = dh.getGuild(new uint[] { userAlly }, helper);
                //CharacterDefID d = new CharacterDefID();
                string guildID = getGuildID(ctx.Member.Id.ToString());
                JObject users;
                JArray user;
                String userID = ctx.Member.Id.ToString();
                String toonName = "";
                try
                {
                    bool added = false, guildFound = false;
                    //if file exists, parse it
                    Console.WriteLine("Reading File");
                    users = JObject.Parse(File.ReadAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\Ignore.txt"));
                    user = (JArray)users["Guilds"];
                    Console.WriteLine("Finding Guild");
                    foreach (JObject obj in user.Children())
                    {
                        Console.WriteLine("Guild Search");
                        if (obj.Property("ID").Value.ToString().Equals(guildID))
                        {
                            Console.WriteLine("Guild Found");
                            guildFound = true;
                            JArray ignored = (JArray)obj[$"Ignored"];
                            if (ignored != null)
                            {
                                Console.WriteLine("Ignored List found");
                                JObject j = (JObject)ignored.SelectToken($"$.[?(@.allycode=='{allycode}')]");
                                if (j != null)
                                {
                                    ignored.Remove(j);
                                    File.WriteAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\Ignore.txt", users.ToString());
                                    Console.WriteLine($"{allycode} has been removed from the ignore list");
                                    dh.logCommandInfo($"{ctx.Member.DisplayName} removed {allycode} from {guildID} Ignore List");
                                    await ctx.RespondAsync($"{allycode} has been removed from your Ignore List");
                                }
                                else
                                {
                                    await ctx.RespondAsync($"{allycode} was not found in your Ignore List");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Ignored Not found"); await ctx.RespondAsync($"{allycode} was not found in your Ignore List");
                            }
                        }
                    }
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                    await m.DeleteAsync();
                }
                catch (FileNotFoundException fnfe)
                {
                    users = new JObject();
                    user = new JArray();
                    JObject guild = new JObject();
                    guild.Add("ID", guildID);
                    JArray ignored = new JArray();
                    JObject ally = new JObject();
                    ally.Add("allycode", allycode);
                    ignored.Add(ally);
                    guild.Add("Ignored", ignored);
                    user.Add(guild);
                    users.Add("Guilds", user);
                    File.WriteAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\Ignore.txt", users.ToString());
                }
                catch (Exception e)
                {
                    await ctx.RespondAsync($"{allycode} not found");
                }

            }

        }
        [Command("twil"), Description("Get the URL to a guilds swgoh.gg profile")]
        public async Task TWIgnoreList(CommandContext ctx)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
            DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
            login();
            uint userAlly = parseAllycode(checkRegistered(ctx.Member.Id.ToString()));

            Console.WriteLine(userAlly);
            //uint code = parseAllycode(allycode);
            JObject toons;
            DataHelper dh = new DataHelper(ctx);
            //  GuildParse.Guild g = dh.getGuild(new uint[] { userAlly }, helper);
            string guildID = getGuildID(ctx.Member.Id.ToString());
            //CharacterDefID d = new CharacterDefID();
            JObject users;
            JArray user;
            String userID = ctx.Member.Id.ToString();
            String ignoreList = "";
            try
            {
                bool added = false, guildFound = false;
                //if file exists, parse it
                Console.WriteLine("Reading File");
                users = JObject.Parse(File.ReadAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\Ignore.txt"));
                user = (JArray)users["Guilds"];
                Console.WriteLine("Finding Guild");
                foreach (JObject obj in user.Children())
                {
                    Console.WriteLine("Guild Search");
                    if (obj.Property("ID").Value.ToString().Equals(guildID))
                    {
                        Console.WriteLine("Guild Found");
                        guildFound = true;
                        JArray ignored = (JArray)obj[$"Ignored"];
                        if (ignored != null)
                        {
                            if (ignored.Count > 0)
                            {
                                Console.WriteLine("Ignored List found");
                                foreach (JObject ignoree in ignored.Children())
                                {
                                    Console.WriteLine("AllyCode: " + ignoree["allycode"]);
                                    ignoreList += $"{ignoree["name"]} - {ignoree["allycode"]}\n\n";
                                }
                            }
                        }
                        else
                        {
                            await ctx.RespondAsync(ctx.Member.Mention + "Your ignore list is empty");
                        }
                    }
                    else
                    {
                        await ctx.RespondAsync(ctx.Member.Mention + "You haven't ignored anyone yet");
                    }
                    break;
                }


                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                await m.DeleteAsync();
                if (ignoreList.Length > 0)
                {
                    await ctx.RespondAsync(ctx.Member.Mention + " Here is your TW ignore List:\n" + ignoreList);
                }
            }
            catch (FileNotFoundException fnfe)
            {
                /* users = new JObject();
                  user = new JArray();
                  JObject guild = new JObject();
                  guild.Add("ID", g.guild[0].Id.ToString());
                  JArray ignored = new JArray();
                  JObject ally = new JObject();
                  ally.Add("allycode", allycode);
                  ignored.Add(ally);
                  guild.Add("Ignored", ignored);
                  user.Add(guild);
                  users.Add("Guilds", user);
                  File.WriteAllText($@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\Data\Ignore.txt", users.ToString());*/
            }
            catch (Exception e)
            {
                //await ctx.RespondAsync($"{allycode} not found");
            }
        }
        [Command("ggprofile"), Description("Get the URL to a guilds swgoh.gg profile")]
        public async Task gettest(CommandContext ctx, [Description("Ally Code to lookup")] uint allycode1)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
            var interactivity = ctx.Client.GetInteractivityModule();
            //String s = "", title = "", embeds = "";
            swgohGGhelper help = new swgohGGhelper();
            string info = help.getGGinfo(allycode1);
            SwgohPlayer p = SwgohPlayer.FromJson(info);

            await ctx.RespondAsync("https://swgoh.gg/g/" + p.Data.GuildId + "/" + p.Data.GuildName.Replace(" ", "-") + "/");
        }
        public uint parseAllycode(string allycode)
        {
            try
            {
                return Convert.ToUInt32(allycode.Replace("-", ""));
            }
            catch (Exception e)
            {
                return 1;
            }
        }
        public uint checkAllycode(CommandContext ctx, string allycode)
        {
            uint ss, setCode;
            if (allycode.Contains("-"))
            {
                setCode = parseAllycode(allycode);
            }
            else
            {
                if (UInt32.TryParse(allycode, out ss))
                {
                    setCode = parseAllycode(ss.ToString());
                }
                else
                {
                    setCode = parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
                }
            }
            return setCode;
        }
        private void login()
        {
            // first, let's load our configuration file
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            var cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
            UserSettings test = new UserSettings();
            test.username = cfgjson.username;
            test.password = cfgjson.password;
            helper = new swgohHelpApiHelper(test);
            if (!helper.loggedIn) { helper.login(); }
        }
        private struct ConfigJson
        {
            [JsonProperty("username")]
            public string username { get; private set; }

            [JsonProperty("password")]
            public string password { get; private set; }
        }
    }
};