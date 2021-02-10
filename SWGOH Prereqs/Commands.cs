using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text.RegularExpressions;
using DSharpPlus.Interactivity;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SWGOH_Prereqs;
using System.Data.SqlClient;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using System.Runtime.ConstrainedExecution;
using System.Timers;
using System.Diagnostics;
using System.Drawing;

namespace SWGOH
{
    /// <summary>
    /// The Commands class. All comands for the bot are defined in here. 
    /// </summary>
    public class Commands
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        CommandFunctions commandFunctions = new CommandFunctions();
        string[] GLList = new string[] { "GLREY", "SUPREMELEADERKYLOREN", "GRANDMASTERLUKE", "SITHPALPATINE" };
        string[] TWToonList = new string[] { "GLREY", "SUPREMELEADERKYLOREN", "GRANDMASTERLUKE", "SITHPALPATINE", "JEDIKNIGHTLUKE", "GENERALSKYWALKER", "DARTHMALAK", "VADER", "PADMEAMIDALA", "JEDIKNIGHTREVAN", "DARTHREVAN", "BASTILASHANDARK", "GRIEVOUS", "DARTHTRAYA", "GEONOSIANBROODALPHA", "CAPITALNEGOTIATOR", "CAPITALMALEVOLENCE", "HOUNDSTOOTH", "MILLENNIUMFALCON" };
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        public swgohHelpApiHelper helper;
        String path = @"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs";
        string[] GAToonList = new string[] { "GLREY", "SUPREMELEADERKYLOREN", "GRANDMASTERLUKE", "SITHPALPATINE", "JEDIKNIGHTLUKE", "GENERALSKYWALKER", "DARTHMALAK", "VADER", "DARTHREVAN", "BASTILASHANDARK", "GRIEVOUS", "PADMEAMIDALA", "JEDIKNIGHTREVAN", "DARTHTRAYA", "ENFYSNEST", "CAPITALNEGOTIATOR", "CAPITALMALEVOLENCE", "HOUNDSTOOTH", "MILLENNIUMFALCON" };
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
                parsedAllyCode = commandFunctions.parseAllycode(allycode);
            }
            else
            {
                if (UInt32.TryParse(allycode, out ss))
                {
                    parsedAllyCode = commandFunctions.parseAllycode(ss.ToString());
                }
                else
                {
                    sort = allycode;
                    parsedAllyCode = commandFunctions.parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
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
                    string sortedBy = "GP DESCENDING";
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                    DiscordMessage m = await ctx.RespondAsync("processing request, standby...");

                    DateTime start = DateTime.Now;
                    // first retrieve the interactivity module from the client
                    var interactivity = ctx.Client.GetInteractivityModule();
                    //login to the API

                    helper = commandFunctions.login();
                    await m.ModifyAsync(m.Content + "\n\nFetching Guild......");
                    String guildstring = dh.getGuildString(new uint[] { parsedAllyCode }, helper);
                    try
                    {
                        //GuildParse.Guild guilds = dh.getGuild(new uint[] { parsedAllyCode }, helper);

                        GuildParse.Guild guilds = JsonConvert.DeserializeObject<GuildParse.Guild>("{\"guild\":" + guildstring + "}");
                        if (guilds.guild.Length > 0)
                        {

                            swgohGGhelper help = new swgohGGhelper();
                            Console.WriteLine(guilds.guild[0].Roster[0].AllyCode);
                            string info = help.getGGinfo(guilds.guild[0].Roster[0].AllyCode);
                            SwgohPlayer p1 = SwgohPlayer.FromJson(info);
                            int maxWidth = 75;
                            string header = $"[{guilds.guild[0].Name}](https://swgoh.gg/g/{p1.Data.GuildId + "/" + p1.Data.GuildName.Replace(" ", "-") + "/"}) Stats ";
                            //string header = guilds.guild[0].Name;
                            int space = maxWidth - header.Length;
                            GuildParse.GuildMember guild = guilds.guild[0];
                            Console.WriteLine(guild.Updated.ToString());
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
                            dh.buildHeader(header, sortedBy, guild, embed);
                            await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                            if (ctx.Guild != null)
                            {
                                await ctx.RespondAsync("", embed: embed);
                                await ctx.RespondAsync("", embed: embed2);
                            }
                            else
                            {
                                await ctx.RespondAsync("", embed: embed);
                                await ctx.RespondAsync("", embed: embed2);
                            }
                        }
                    }
                    catch (Exception errors)
                    {
                        Console.WriteLine(errors.StackTrace);
                        GuildParse.Error guilds = JsonConvert.DeserializeObject<GuildParse.Error>(guildstring);
                        if (guilds.Code == 404)
                        {
                            embed.Title = "Oh No!";
                            embed.Description = "Guild not found with that allycode. Please try another.";
                            await ctx.RespondAsync("", embed: embed);
                        }
                    }
                    DateTime end = DateTime.Now;
                    Console.WriteLine((end - start).TotalSeconds);
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                    await m.DeleteAsync();

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
                parsedAllyCode = commandFunctions.parseAllycode(allycode);
            }
            else
            {
                if (UInt32.TryParse(allycode, out ss))
                {
                    parsedAllyCode = commandFunctions.parseAllycode(ss.ToString());
                }
                else
                {
                    parsedAllyCode = commandFunctions.parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
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
                    helper = commandFunctions.login();

                    await m.ModifyAsync(m.Content + "\n\n Fetching guild");
                    try
                    {
                        GuildParse.Guild guilds = dh.getGuild(new uint[] { parsedAllyCode }, helper);
                        GuildParse.GuildMember guild = guilds.guild[0];
                        swgohGGhelper help = new swgohGGhelper();
                        bool ggFound = false;
                        SwgohPlayer p1 = null;
                        try
                        {
                            string info = help.getGGinfo(guild.Roster[0].AllyCode);
                            p1 = SwgohPlayer.FromJson(info);
                            ggFound = true;
                        }
                        catch
                        {

                        }

                        if (guilds.guild.Length > 0)
                        {
                            int maxWidth = 75;
                            string header = "";
                            if (ggFound)
                            {
                                header = $"[{guild.Name}](https://swgoh.gg/g/{p1.Data.GuildId + "/" + p1.Data.GuildName.Replace(" ", "-") + "/"}) Overview ";
                            }
                            else
                            {
                                header = $"{guild.Name} Overview ";
                            }
                            int space = maxWidth - header.Length;
                            await m.ModifyAsync(m.Content + "\n\n Building stats display");
                            var embed2 = new DiscordEmbedBuilder
                            {
                                Title = "Stats",
                                Color = new DiscordColor(0xFF0000) // red
                            };
                            /* s += "**" + header + "**\n";
                             s += "======= Overview =======```\n";
                             s += dh.createLine("Members:", guilds.guild[0].Members.ToString());
                             s += dh.createLine("Total GP:", (guilds.guild[0].Gp / 1000000.0).ToString("0.##") + "M");
                             s += dh.createLine("Character GP:", (dh.buildCharGP(guilds.guild[0].Roster, "Char") / 1000000.0).ToString("###.##") + "M");
                             s += dh.createLine("Fleet GP:", (dh.buildCharGP(guilds.guild[0].Roster, "Fleet") / 1000000.0).ToString("###.##") + "M");
                             s += "```";
                             embed.Description = s;*/
                            dh.buildHeader(header, "", guild, embed);
                            await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                            if (ctx.Guild != null) { await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:", embed: embed); }
                            else { await ctx.RespondAsync(" Here is the information you requested:", embed: embed); }
                        }
                        DateTime end = DateTime.Now;
                        Console.WriteLine((end - start).TotalSeconds);
                        await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                        await m.DeleteAsync();
                    }
                    catch (Exception e)
                    {
                        await m.DeleteAsync();
                        await ctx.RespondAsync("There was an error prcessing your request.");
                        dh.logCommandInfo($"go command error: {e.Source}\n\n{e.Message}\n\n{e.StackTrace}");

                    }
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
                helper = commandFunctions.login();
                await registerUser(ctx, commandFunctions.parseAllycode(allycode));
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
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
        public async Task guildChar(CommandContext ctx, [RemainingText][Description("Toon to lookup in the guild")] string toon)
        {

            uint sd;
            uint parsedAllyCode;

            parsedAllyCode = commandFunctions.parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
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
                        string toonName = readToons(ctx, toon);
                        DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
                        if (!toonName.Equals(""))
                        {
                            DateTime start = DateTime.Now;
                            // first retrieve the interactivity module from the client
                            var interactivity = ctx.Client.GetInteractivityModule();

                            String s = "";
                            //login to the API
                            helper = commandFunctions.login();
                            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));

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
                                    if (players1 != null)
                                    {
                                        s += "**" + header + "**\n";
                                        embed.Description = s;
                                        await m.ModifyAsync("Building output..");
                                        dh.guildChar(players1, ctx, embed, toonName);
                                    }
                                    else
                                    {
                                        String embeds = "";
                                        s += "**" + header + "**\n";
                                        embed.Description = s;
                                        embeds += "```CSS\n";
                                        embeds += "Unable to retrieve the data request, please try again later.";
                                        embeds += "```";
                                        embed.AddField($"Error", embeds, false);

                                    }
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
                                await ctx.RespondAsync(ctx.User.Mention + " there was an API issue retrieving your guild.");
                            }
                        }
                        else
                        {
                            string embeds = "";
                            embed.Description = "Error retrieving data";
                            embeds += "```CSS\n";
                            embeds += "Unable to retrieve the data request, please try again later.";
                            embeds += "```";
                            embed.AddField($"Error", embeds, false);
                            await ctx.RespondAsync(ctx.Member.Mention + "", embed: embed);
                            await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));

                            await m.DeleteAsync();
                        }
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

                    readToonsList(ctx, embed);
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
                            if (c.Name.Equals("tw") || c.Name.Equals("gs") || c.Name.Equals("reg") || c.Name.Equals("checkusers") || c.Name.Equals("help") || c.Name.Equals("gac") || c.Name.Equals("go") || c.Name.Equals("unreg") || c.Name.Equals("twir") || c.Name.Equals("twi") || c.Name.Equals("twil") || c.Name.Equals("twic"))
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
        [Command("tw"), Description("Compares 2 guilds for TW")]
        public async Task TWCompare(CommandContext ctx, [Description("Ally code of one guild")] string allycode1, [Description("Ally Code of other guild")] string allycode2 = "")
        {
            uint parsedAllyCode1 = 1, parsedAllyCode2 = 1;
            if (allycode2.Equals(""))
            {
                parsedAllyCode2 = commandFunctions.parseAllycode(commandFunctions.checkRegistered(ctx.Member.Id.ToString()));
                parsedAllyCode1 = commandFunctions.checkAllycode(ctx, allycode1);
            }
            else
            {
                parsedAllyCode1 = commandFunctions.checkAllycode(ctx, allycode1);
                parsedAllyCode2 = commandFunctions.checkAllycode(ctx, allycode2);
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

        #region TW Compare Logic
        public async void getTWCompare(CommandContext ctx, uint allycode1, uint allycode2)
        {
            Console.WriteLine($"Allycode 1: {allycode1} ::  Allycode 2:{allycode2}");
            if (allycode1 != allycode2)
            {
                Stopwatch stop = new Stopwatch();
                stop.Start();
                //DateTime start = DateTime.Now;
                DataHelper dh = new DataHelper(ctx);
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                DiscordMessage m = await ctx.RespondAsync("processing request, standby...");

                var interactivity = ctx.Client.GetInteractivityModule();
                String s = "", title = "", embeds = "";

                //login to the API
                helper = commandFunctions.login();
                //Retreive the 2 guilds to compare using an allycode for a member of each guild
                //If both allycodes are from the same guild, only one guild will return, make sure to check for that
                if (helper.loggedIn)
                {
                    int tries = 0;
                    GuildParse.Guild guilds = null;
                    while (tries < 5)
                    {
                        Console.WriteLine($"Allycode 1: {allycode1}  &&  Allycode 2: {allycode2}");
                        try
                        {
                            await m.ModifyAsync(m.Content + $"\n\n....attempting to retrieve guilds. Try {tries + 1}/5");
                            guilds = dh.getGuild(new uint[] { allycode1, allycode2 }, helper);
                            await m.ModifyAsync($"\n\n....Data retrieved.\n\n");
                            break;
                        }
                        catch { tries++; }
                    }
                    string totalEmbeds = "";
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
                                    for (int j = 0; j < players1.PlayerList.Length; j++)
                                    {
                                        try
                                        {
                                            swgohGGhelper help = new swgohGGhelper();
                                            string info = help.getGGinfo((uint)players1.PlayerList[j].AllyCode);
                                            SwgohPlayer p1 = SwgohPlayer.FromJson(info);
                                            link1 = $"https://swgoh.gg/g/{p1.Data.GuildId + "/" + p1.Data.GuildName.Replace(" ", "-") + "/"}";
                                            break;
                                        }
                                        catch { }
                                    }
                                    for (int k = 0; k < players2.PlayerList.Length; k++)
                                    {
                                        try
                                        {
                                            swgohGGhelper help = new swgohGGhelper();
                                            string info = help.getGGinfo((uint)players2.PlayerList[k].AllyCode);
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
                                    try
                                    {
                                        dh.calcIgnoredGP(guild1, guild1st);
                                    }
                                    catch { }
                                    try
                                    {
                                        dh.calcIgnoredGP(guild2, guild2nd);
                                    }
                                    catch { }
                                    //Build our output
                                    #region Header Output
                                    s += "**" + header + "**\n";
                                    s += "======= Overview =======```\n";
                                    try
                                    {
                                        s += dh.createLongLine("Members:", (guild1.Members - ((guild1st.ignoreList != null) ? guild1st.ignoreList.Count : 0)).ToString(), (guild2.Members - ((guild2nd.ignoreList != null) ? guild2nd.ignoreList.Count : 0)).ToString());
                                    }
                                    catch { s += dh.createLongLine("Members:", guild1.Members.ToString(), guild2.Members.ToString()); }
                                    s += dh.createLongLine("Total GP:", ((guild1.Gp - guild1st.gpIgnored) / 1000000).ToString("0.##") + "M", ((guild2.Gp - guild2nd.gpIgnored) / 1000000).ToString("0.##") + "M");
                                    s += dh.createLongLine("Character GP:", ((dh.buildCharGP(guild1.Roster, "Char") - guild1st.gpIgnoredToon) / 1000000).ToString("0.##") + "M", ((dh.buildCharGP(guild2.Roster, "Char") - guild2nd.gpIgnoredToon) / 1000000).ToString("0.##") + "M");
                                    s += dh.createLongLine("Fleet GP:", ((dh.buildCharGP(guild1.Roster, "Fleet") - guild1st.gpIgnoredFleet) / 1000000).ToString("0.##") + "M", ((dh.buildCharGP(guild2.Roster, "Fleet") - guild2nd.gpIgnoredFleet) / 1000000).ToString("0.##") + "M");
                                    s += "```";
                                    embed.Description = s;
                                    // totalEmbeds += s;
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
                                    totalEmbeds += embeds;
                                    #endregion
                                    #region relics
                                    title = "==Relics==";
                                    embeds = "```CSS\n";
                                    embeds += dh.createLongLine("Total Relics:", guild1st.TotalRelics.ToString(), guild2nd.TotalRelics.ToString());
                                    embeds += dh.createLongLine("Relic 8:", guild1st.relics[8].ToString(), guild2nd.relics[8].ToString());
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
                                    totalEmbeds += embeds;
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
                                    totalEmbeds += embeds;
                                    #endregion
                                    CharacterDefID d = new CharacterDefID();
                                    int totalGL1 = 0, totalGL2 = 0;
                                    foreach (string toonName in GLList)
                                    {
                                        Toons p1, p2;
                                        p1 = guild1st.toonList.Find(x => x.id.Equals(toonName));
                                        if (p1 == null) { /*p1 = new Toons(toonName, 6); /*guild1st.toonList.Add(p1);*/ }
                                        p2 = guild2nd.toonList.Find(x => x.id.Equals(toonName));
                                        if (p2 == null) { /*p2 = new Toons(toonName, 6); guild2nd.toonList.Add(p2); */}
                                        totalGL1 += p1.Total;
                                        totalGL2 += p2.Total;
                                    }
                                    title = $"GL Totals";
                                    embeds = "```CSS\n";
                                    embeds += dh.createLongLine("Total GL:", totalGL1.ToString(), totalGL2.ToString());
                                    foreach (string toonName in GLList)
                                    {
                                        Toons p1, p2;
                                        p1 = guild1st.toonList.Find(x => x.id.Equals(toonName));
                                        if (p1 == null) { /*p1 = new Toons(toonName, 6); /*guild1st.toonList.Add(p1);*/ }
                                        p2 = guild2nd.toonList.Find(x => x.id.Equals(toonName));
                                        if (p2 == null) { /*p2 = new Toons(toonName, 6); guild2nd.toonList.Add(p2); */}

                                        embeds += dh.createLongLine($"Total {d.toons[toonName]}:", p1.Total.ToString(), p2.Total.ToString());
                                    }
                                    embeds += "```";
                                    totalEmbeds += embeds;
                                    embed.AddField($"{title}", embeds, false);
                                    foreach (string toonName in TWToonList)
                                    {
                                        Toons p1, p2;
                                        //Look for the desired toon in the players list, if it exists, the corresponding GAToon will be a copy of it, otherwise it will be null
                                        p1 = guild1st.toonList.Find(x => x.id.Equals(toonName));
                                        if (p1 == null) { p1 = new Toons(toonName, 6); guild1st.toonList.Add(p1); }
                                        p2 = guild2nd.toonList.Find(x => x.id.Equals(toonName));
                                        if (p2 == null) { p2 = new Toons(toonName, 6); guild2nd.toonList.Add(p2); }

                                        //if a particular toon is not in the players list, create a new empty toon for comparison reasons
                                        title = $"{d.toons[toonName]}";
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
                                                if (toonName.Equals("GLREY") || toonName.Equals("SUPREMELEADERKYLOREN") || toonName.Equals("GRANDMASTERLUKE") || toonName.Equals("SITHPALPATINE") || toonName.Equals("GENERALSKYWALKER") || toonName.Equals("DARTHREVAN") || toonName.Equals("JEDIKNIGHTREVAN") || toonName.Equals("VADER"))
                                                {
                                                    embeds += dh.createLine("3Z:", p1.numZetas[2].ToString(), p2.numZetas[2].ToString());
                                                }
                                                if (toonName.Equals("GLREY") || toonName.Equals("SUPREMELEADERKYLOREN") || toonName.Equals("GRANDMASTERLUKE") || toonName.Equals("SITHPALPATINE") || toonName.Equals("GENERALSKYWALKER"))
                                                {
                                                    embeds += dh.createLine("4Z:", p1.numZetas[3].ToString(), p2.numZetas[3].ToString());
                                                }
                                                if (toonName.Equals("GLREY") || toonName.Equals("SUPREMELEADERKYLOREN") || toonName.Equals("GRANDMASTERLUKE") || toonName.Equals("SITHPALPATINE"))
                                                {
                                                    embeds += dh.createLine("5Z:", p1.numZetas[4].ToString(), p2.numZetas[4].ToString());
                                                    embeds += dh.createLine("6Z:", p1.numZetas[5].ToString(), p2.numZetas[5].ToString());
                                                }
                                            }
                                        }
                                        embeds += "```";
                                        embed.AddField($"{title}", embeds, true);
                                        totalEmbeds += embeds;
                                    }
                                    #region ignoring
                                    try
                                    {
                                        Console.WriteLine("Building Ignore embed");
                                        String list = "";
                                        try
                                        {
                                            if (guild1st.ignoreList != null)
                                            {
                                                foreach (String member in guild1st.ignoreList)
                                                {
                                                    foreach (GuildParse.Roster r in guild1.Roster)
                                                    {
                                                        if (r.AllyCode.ToString().Equals(member))
                                                        {
                                                            try
                                                            {
                                                                list += $"{r.Name} - {member}  \n\n";
                                                                Console.WriteLine($"{member} added to ignore embed");
                                                            }
                                                            catch { }
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
                                        catch { }
                                        try
                                        {
                                            if (guild2nd.ignoreList != null)
                                            {
                                                foreach (String member in guild2nd.ignoreList)
                                                {
                                                    foreach (GuildParse.Roster r in guild2.Roster)
                                                    {
                                                        if (r.AllyCode.ToString().Equals(member))
                                                        {
                                                            try
                                                            {
                                                                list += $"{r.Name} - {member}  \n\n";
                                                                Console.WriteLine($"{member} added to ignore embed");
                                                            }
                                                            catch { }
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
                                        catch { }
                                    }
                                    catch { Console.WriteLine("Building Ignore embed failed"); }
                                    #endregion

                                    stop.Stop();
                                    Console.WriteLine(stop.Elapsed.ToString("G"));
                                    Console.WriteLine(totalEmbeds);

                                    ImageFromText i = new ImageFromText();
                                    string image = $"{guild1.Name}.png";
                                    totalEmbeds = totalEmbeds.Replace("```", "").Replace(".", " ");
                                    // i.DrawText(totalEmbeds, new Font("Arial", 96, FontStyle.Bold), System.Drawing.Color.Black, 3200, image);
                                    //  await ctx.RespondWithFileAsync(image);
                                    // DateTime end = DateTime.Now;
                                    //Console.WriteLine((end - start).TotalSeconds);

                                    await m.DeleteAsync();
                                    try
                                    {
                                        Console.WriteLine("trying to respond");
                                        await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:", embed: embed);

                                    }
                                    catch
                                    {
                                        Console.WriteLine("Something is goofy");
                                        string desc = embed.Description;
                                        foreach (DiscordEmbedField f in embed.Fields)
                                        {
                                            Console.WriteLine(f.Name + "   ");
                                        }
                                        embed.Description = desc.Replace("[", "").Replace("]", "").Replace($"({link1})", "").Replace($"({link2})", "");
                                        await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:", embed: embed);
                                    }
                                    dh.logCommandInformation("Successful");
                                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
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
                            await m.ModifyAsync($"\n\n....Only 1 guild found.......Processing.\n\n");
                            bool match = false; uint badCode = 0;
                            foreach (GuildParse.Roster member in guilds.guild[0].Roster)
                            {
                                if (allycode1 == member.AllyCode)
                                {
                                    match = true;
                                    badCode = allycode2;
                                    await m.ModifyAsync($"\n\n....Found Bad Code.\n\n");
                                    break;
                                }
                            }
                            if (!match)
                            {
                                badCode = allycode1;
                            }

                            String guildstring = dh.getGuildString(new uint[] { badCode }, helper);
                            var embed = new DiscordEmbedBuilder
                            {
                                Title = "Stats",
                                Color = new DiscordColor(0xFF0000) // red
                            };
                            GuildParse.Error guildTester = JsonConvert.DeserializeObject<GuildParse.Error>(guildstring);
                            if (guildTester.Code == 404)
                            {
                                Console.WriteLine("Here");
                                embed.Title = "Oh No!";
                                embed.Description = $"Guild not found with  ***{badCode}***. Please try another.";
                                await ctx.RespondAsync("", embed: embed);
                            }
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
            else
            {
                await ctx.RespondAsync(ctx.User.Mention + " please supply 2 valid or different codes.");

            }
        }
        #endregion
        [Command("twi"), Description("Add a guild member to the ignore list for TW. This list is set for the guild, NOT the user.")]
        public async Task TWignore(CommandContext ctx, [RemainingText] string allycode)
        {
            Console.WriteLine(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
            string[] ignoredCodes;
            uint[] parsedAllycodes;
            try
            {
                ignoredCodes = allycode.Split(' ');
                parsedAllycodes = new uint[ignoredCodes.Length];
            }
            catch (Exception e) { Console.WriteLine(e.Message); ignoredCodes = new string[1]; ignoredCodes[0] = allycode; parsedAllycodes = new uint[ignoredCodes.Length]; }
            Console.WriteLine(ignoredCodes.Length);
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
            DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
            uint parsedAllyCode, sd;

            int i = 0;
            foreach (string s in ignoredCodes)
            {
                if (s.Contains("-"))
                {
                    parsedAllycodes[i] = commandFunctions.parseAllycode(s.Replace("-", ""));
                }
                else
                {
                    if (UInt32.TryParse(s, out sd))
                    {
                        parsedAllycodes[i] = commandFunctions.parseAllycode(sd.ToString());
                    }
                    else
                    {
                        await ctx.RespondAsync($"{s} is not a valid allycode");
                    }
                }
                i++;
            }
            Console.WriteLine(parsedAllycodes.Length);
            helper = commandFunctions.login();
            uint userAlly = commandFunctions.parseAllycode(commandFunctions.checkRegistered(ctx.Member.Id.ToString()));
            await m.ModifyAsync("acquired allycode(s)");
            if ((parsedAllycodes.Length > 0))
            {
                //uint code = parseAllycode(allycode);
                JObject toons;
                DataHelper dh = new DataHelper(ctx);
                await m.ModifyAsync(m.Content + "\n\nRetirieving Guild");
                GuildParse.Guild g = dh.getGuild(new uint[] { userAlly }, helper);
                String guildID = getGuildID(ctx.Member.Id.ToString());
                //CharacterDefID d = new CharacterDefID();
                JObject users;
                JArray user;
                String userID = ctx.Member.Id.ToString();
                String toonName = "";
                try
                {
                    bool added = false, guildFound = false, inGuild = false;
                    //if file exists, parse it
                    await m.ModifyAsync("Comparing to roster");
                    Console.WriteLine("Reading File");
                    users = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/Ignore.txt"));
                    user = (JArray)users["Guilds"];
                    Console.WriteLine("Finding Guild");
                    await m.ModifyAsync(m.Content + "\nChecking ignore list");
                    foreach (JObject obj in user.Children())
                    {
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
                            await m.ModifyAsync("ignore list found, checking against provided code(s)");
                            foreach (uint code in parsedAllycodes)
                            {
                                if (code > 0)
                                {
                                    JObject j = (JObject)ignored.SelectToken($"$.[?(@.allycode=={code})]");
                                    if (j == null)
                                    {
                                        Console.WriteLine($"{code} not in list");
                                        JObject newToon = new JObject();
                                        newToon.Add("allycode", code);
                                        foreach (GuildParse.Roster r in g.guild[0].Roster)
                                        {
                                            if (code.ToString().Equals(r.AllyCode.ToString())) { newToon.Add("name", r.Name); inGuild = true; }
                                        }
                                        if (inGuild)
                                        {
                                            ignored.Add(newToon);
                                            dh.logCommandInfo($"{ctx.Member.DisplayName} added {code} to {g.guild[0].Id.ToString()} Ignore List");
                                            await ctx.RespondAsync($"{code} has been added to your Ignore List");
                                            inGuild = false;
                                        }
                                        else
                                        {
                                            await ctx.RespondAsync($"{code} is not in your guild");
                                        }
                                    }
                                    else
                                    {
                                        await ctx.RespondAsync($"{code} was already in your Ignore List");
                                    }
                                }
                            }
                        }
                    }
                    if (!guildFound)
                    {
                        JObject guild = new JObject();
                        guild.Add("ID", g.guild[0].Id.ToString());
                        user.Add(guild);
                        JArray ignored = new JArray();
                        guild.Add("Ignored", ignored);
                        JObject newIgnore = new JObject();
                        foreach (uint code in parsedAllycodes)
                        {
                            newIgnore.Add("allycode", code);
                            foreach (GuildParse.Roster r in g.guild[0].Roster)
                            {
                                if (code.ToString().Equals(r.AllyCode.ToString())) { newIgnore.Add("name", r.Name); }
                            }
                            ignored.Add(newIgnore);
                        }
                        //  File.WriteAllText((RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "" : $@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs") + @"\Data\Ignore.txt", users.ToString());
                    }
                    /*else
                    {
                        if (added)
                        {
                          //  File.WriteAllText((RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "" : $@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs") + @"\Data\Ignore.txt", users.ToString());
                            //dh.logCommandInfo($"{ctx.Member.DisplayName} added {parsedAllyCode} to {g.guild[0].Id.ToString()} Ignore List");
                            await ctx.RespondAsync($"{parsedAllyCode} has been added to your Ignore List");
                        }
                        else
                            await ctx.RespondAsync($"{parsedAllyCode} was already in your Ignore List");
                    }*/
                    File.WriteAllText(commandFunctions.detectOS() + @"Data/Ignore.txt", users.ToString());
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                    await m.DeleteAsync();
                }
                catch (FileNotFoundException fnfe)
                {
                    /* users = new JObject();
                     user = new JArray();
                     JObject guild = new JObject();
                     guild.Add("ID", g.guild[0].Id.ToString());
                     JArray ignored = new JArray();
                     JObject ally = new JObject();
                     ally.Add("allycode", parsedAllyCode);
                     ignored.Add(ally);
                     guild.Add("Ignored", ignored);
                     user.Add(guild);
                     users.Add("Guilds", user);
                     File.WriteAllText((RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "" : $@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs") + @"\Data\Ignore.txt", users.ToString());*/
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "::::" + e.StackTrace);
                    await ctx.RespondAsync($"{allycode} not found");
                }

            }

        }
        #region TW Orders
        [Command("two"), Description("Territory War Orders"), Hidden]
        public async Task getOrders(CommandContext ctx)
        {
            DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
            string guildID = getGuildID(ctx.Member.Id.ToString());
            JObject users;
            JArray user;
            String userID = ctx.Member.Id.ToString();
            String toonName = "";

            List<String> ignoredList = new List<string>();
            await m.ModifyAsync("Checking Ignore List");
            try
            {
                bool added = false, guildFound = false;
                //if file exists, parse it
                Console.WriteLine("Reading File");
                users = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/Ignore.txt"));
                user = (JArray)users["Guilds"];
                Console.WriteLine("Finding Guild");
                foreach (JObject obj in user.Children())
                {
                    if (obj.Property("ID").Value.ToString().Equals(guildID))
                    {
                        Console.WriteLine("Guild Found");
                        guildFound = true;
                        JArray ignored = (JArray)obj[$"Ignored"];
                        if (ignored != null)
                        {
                            Console.WriteLine("Ignored List found");
                            foreach (JObject t in ignored.Children())
                            {
                                ignoredList.Add(t.Property("name").Value.ToString());
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException fnfe)
            {

            }

            await m.ModifyAsync("Ignore list processed. Retrieving orders");
            string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
            string ApplicationName = "TW Orders";
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String spreadsheetId = "1Jrjn84StqflzVvS3DGXehke2jCDQfq7UJBJiZSNf0uY";
            String range = "Sheet2!A2:Z";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            string assignments = "";
            var embed = new DiscordEmbedBuilder
            {
                Title = "TW Comparison",
                Color = new DiscordColor(0xFF0000) // red
            };
            DiscordRole dr = ctx.Guild.EveryoneRole;
            foreach (var names in ctx.Guild.Roles)
            {
                if (names.Name.Equals("Ewoks"))
                {
                    dr = names;
                }
            }
            await m.ModifyAsync("Orders found. Building Display");
            string embeds = "", title = "";
            Console.WriteLine(ctx.Guild.MemberCount);
            IReadOnlyList<DiscordMember> mems = await ctx.Guild.GetAllMembersAsync();
            foreach (var member in mems)
            {
                Console.WriteLine(member.Id);
            }
            if (values != null && values.Count > 0)
            {
                List<DiscordEmbedBuilder> indOrders = new List<DiscordEmbedBuilder>();
                try
                {
                    foreach (var row in values)
                    {
                        if (!ignoredList.Contains(row[0]))
                        {
                            embeds = "```diff\n";
                            title = $"{row[0]} Assignments";

                            int j = 0;
                            embeds += $"{row[2]} Def Banners\n";
                            embeds += "+ Defensive Squads\n";

                            foreach (var assignment in row)
                            {
                                if (j != 0 && j != 1 && j != 2)
                                {
                                    if (Regex.IsMatch(assignment.ToString(), @"\d{2,6}") || !Regex.IsMatch(assignment.ToString(), @"\d{1,1}") || Regex.IsMatch(assignment.ToString(), @"\b(\w*fleet defense\w*)\b"))
                                    {
                                        embeds += "- ";
                                    }
                                    if (j != row.Count - 1)
                                        embeds += assignment + "\n";
                                    else
                                        embeds += assignment;
                                }
                                j++;
                            }
                            embeds += "```";
                            try
                            {
                                foreach (var member in ctx.Guild.Members)
                                {
                                    if (member.Id.ToString().Equals(row[1].ToString().Replace("<@", "").Replace("!", "").Replace(">", "")))
                                    {
                                        var individualEmbed = new DiscordEmbedBuilder
                                        {
                                            Title = "TW Assignment",
                                            Color = new DiscordColor(0xFF0000) // red
                                        };
                                        individualEmbed.AddField($"{title}", row[1] + "\n" + embeds);
                                        indOrders.Add(individualEmbed);
                                    }
                                }
                                embed.AddField($"{title}", row[1] + "\n" + embeds, true);
                            }
                            catch
                            {
                                await ctx.RespondAsync(dr.Mention + " Here are the TW Assignments:", embed: embed);
                                embed = new DiscordEmbedBuilder
                                {
                                    Title = "",
                                    Color = new DiscordColor(0xFF0000) // red
                                };
                                embed.AddField($"{title}", row[1] + "\n" + embeds, true);
                            }
                            j++;
                        }
                    }

                    String ignoredPlayers = "";
                    try
                    {
                        //foreach (string name in ignoredList) { ignoredPlayers += name + ","; }
                       // ignoredPlayers = ignoredPlayers.Substring(0, ignoredPlayers.Length - 1);
                        //embed.AddField($"Ignoring:", $"```{ignoredPlayers}```", false);
                        await ctx.RespondAsync(embed: embed);
                    }
                    catch
                    {
                        await ctx.RespondAsync(embed: embed);
                        embed = new DiscordEmbedBuilder
                        {
                            Title = "",
                            Color = new DiscordColor(0xFF0000) // red
                        };
                       embed.AddField($"Ignoring:", $"```{ignoredPlayers}```", false);
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                await m.DeleteAsync();
                int DMs = 0;
                foreach (DiscordEmbedBuilder emb in indOrders)
                {
                    foreach (var member in ctx.Guild.Members)
                    {
                        if (emb.Fields[0].Value.Contains(member.Id.ToString()))
                        {
                            try
                            {
                                await member.SendMessageAsync("", embed: emb);
                                Thread.Sleep(750);
                                DMs++;
                            }
                            catch { Console.WriteLine($"{member.DisplayName} broke the message"); }
                        }
                    }
                }
                Console.WriteLine($"{DMs} sent");
            }
        }

        #endregion
        [Command("twir"), Description("Remove a member from the TW ignore list.")]
        public async Task TWremoveIgnore(CommandContext ctx, string allycode)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
            DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
            uint parsedAllyCode, sd;
            if (allycode.Contains("-"))
            {
                parsedAllyCode = commandFunctions.parseAllycode(allycode);
            }
            else
            {
                if (UInt32.TryParse(allycode, out sd))
                {
                    parsedAllyCode = commandFunctions.parseAllycode(sd.ToString());
                }
                else
                {
                    parsedAllyCode = commandFunctions.parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
                }
            }
            helper = commandFunctions.login();
            uint userAlly = commandFunctions.parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
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
                    users = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/Ignore.txt"));
                    user = (JArray)users["Guilds"];
                    Console.WriteLine("Finding Guild");
                    foreach (JObject obj in user.Children())
                    {
                        if (obj.Property("ID").Value.ToString().Equals(guildID))
                        {
                            Console.WriteLine("Guild Found");
                            guildFound = true;
                            JArray ignored = (JArray)obj[$"Ignored"];
                            if (ignored != null)
                            {
                                Console.WriteLine("Ignored List found");
                                JObject j = (JObject)ignored.SelectToken($"$.[?(@.allycode=='{parsedAllyCode}')]");
                                if (j != null)
                                {
                                    ignored.Remove(j);
                                    File.WriteAllText(commandFunctions.detectOS() + @"Data/Ignore.txt", users.ToString());
                                    Console.WriteLine($"{parsedAllyCode} has been removed from the ignore list");
                                    dh.logCommandInfo($"{ctx.Member.DisplayName} removed {parsedAllyCode} from {guildID} Ignore List");
                                    await ctx.RespondAsync($"{parsedAllyCode} has been removed from your Ignore List");
                                }
                                else
                                {
                                    await ctx.RespondAsync($"{parsedAllyCode} was not found in your Ignore List");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Ignored Not found"); await ctx.RespondAsync($"{parsedAllyCode} was not found in your Ignore List");
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
                    ally.Add("allycode", parsedAllyCode);
                    ignored.Add(ally);
                    guild.Add("Ignored", ignored);
                    user.Add(guild);
                    users.Add("Guilds", user);
                    File.WriteAllText(commandFunctions.detectOS() + @"Data/Ignore.txt", users.ToString());
                }
                catch (Exception e)
                {
                    await ctx.RespondAsync($"{parsedAllyCode} not found");
                }

            }

        }
        [Command("twic"), Description("Clear all allycodes from the TW Ignore list.")]
        public async Task TWremoveClear(CommandContext ctx)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
            DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
            helper = commandFunctions.login();
            uint userAlly = commandFunctions.parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
            DataHelper dh = new DataHelper(ctx);
            string guildID = getGuildID(ctx.Member.Id.ToString());
            JObject users;
            JArray user;
            String userID = ctx.Member.Id.ToString();
            try
            {
                bool added = false, guildFound = false;
                //if file exists, parse it
                Console.WriteLine("Reading File");
                users = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/Ignore.txt"));
                user = (JArray)users["Guilds"];
                Console.WriteLine("Finding Guild");
                foreach (JObject obj in user.Children())
                {
                    if (obj.Property("ID").Value.ToString().Equals(guildID))
                    {
                        Console.WriteLine("Guild Found");
                        guildFound = true;
                        JArray ignored = (JArray)obj[$"Ignored"];
                        if (ignored != null)
                        {
                            Console.WriteLine("Ignored List found");
                            obj.Remove("Ignored");
                            File.WriteAllText(commandFunctions.detectOS() + @"Data/Ignore.txt", users.ToString());
                            Console.WriteLine("Ignored List Cleared"); await ctx.RespondAsync($"Your ignore list has been cleared");
                        }
                        else
                        {
                            Console.WriteLine("Ignored Not found"); await ctx.RespondAsync($"Your ignore list was already empty");
                        }
                    }
                }
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                await m.DeleteAsync();
            }
            catch (FileNotFoundException fnfe)
            {
            }
            catch (Exception e)
            {
            }
        }
        [Command("twil"), Description("Show the TW Ignore list")]
        public async Task TWIgnoreList(CommandContext ctx)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
            DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
            helper = commandFunctions.login();
            uint userAlly = commandFunctions.parseAllycode(checkRegistered(ctx.Member.Id.ToString()));

            JObject toons;
            DataHelper dh = new DataHelper(ctx);
            string guildID = getGuildID(ctx.Member.Id.ToString());
            JObject users;
            JArray user;
            String userID = ctx.Member.Id.ToString();
            String ignoreList = "";
            try
            {
                bool added = false, guildFound = false;
                //if file exists, parse it
                Console.WriteLine("Reading File");
                users = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/Ignore.txt"));
                user = (JArray)users["Guilds"];
                Console.WriteLine("Finding Guild");
                foreach (JObject obj in user.Children())
                {
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
                            await ctx.RespondAsync(ctx.Member.Mention + "You haven't ignored anyone yet");
                        }
                        break;
                    }
                }



                await m.DeleteAsync();
                if (ignoreList.Length > 0)
                {
                    await ctx.RespondAsync(ctx.Member.Mention + " Here is your TW ignore List:\n" + ignoreList);
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
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
                Console.WriteLine(e.StackTrace);
            }
        }
        #region GA Compare Logic
        [Command("gac"), Description("Compare 2 players for GAC")]
        public async Task getGAC(CommandContext ctx, [Description("Ally Code to lookup")] string allycode1, string allycode2 = "")
        {
            uint parsedAllyCode1 = 1, parsedAllyCode2 = 1;
            if (allycode2.Equals(""))
            {
                parsedAllyCode2 = commandFunctions.parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
                parsedAllyCode1 = commandFunctions.checkAllycode(ctx, allycode1);
            }
            else
            {
                parsedAllyCode1 = commandFunctions.checkAllycode(ctx, allycode1);
                parsedAllyCode2 = commandFunctions.checkAllycode(ctx, allycode2);
            }
            if ((!(parsedAllyCode1 == 1) && !(parsedAllyCode2 == 1)))
            {
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
                var interactivity = ctx.Client.GetInteractivityModule();
                String s = "", title = "", embeds = "";
                DataHelper dh = new DataHelper(ctx);
                helper = commandFunctions.login();
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
                        title = $"{d.toons[toonName]}";
                        embeds = "```CSS\n";
                        embeds += dh.createShortLine("GP:", p1.GP.ToString(), p2.GP.ToString());
                        embeds += dh.createShortLine("Stars:", p1.starLevel.ToString(), p2.starLevel.ToString());
                        embeds += dh.createShortLine("Speed:", p1.speed.ToString(), p2.speed.ToString());
                        if (!p1.isShip && !p2.isShip)
                        {
                            embeds += dh.createShortLine("Gear:", p1.gearLevel.ToString() + (p1.gearEquipped > 0 ? $"+{p1.gearEquipped}" : ""), p2.gearLevel.ToString() + (p2.gearEquipped > 0 ? $"+{p2.gearEquipped}" : ""));
                            embeds += dh.createShortLine("Relics:", p1.relic.ToString(), p2.relic.ToString());
                            embeds += dh.createShortLine("Zetas:", p1.Zetas.ToString(), p2.Zetas.ToString());
                            embeds += dh.createShortLine("P Dam:", p1.physDam.ToString(), p2.physDam.ToString());
                            embeds += dh.createShortLine("S Dam:", p1.specDam.ToString(), p2.specDam.ToString());
                        }
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
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
                bool found = false;
                //if file exists, parse it
                users = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/users.txt"));
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
                    File.WriteAllText(commandFunctions.detectOS() + @"Data/users.txt", users.ToString());
                    await ctx.RespondAsync($"<@{userID}> has unregistered. Register with a new allycode using ```;;r allycode```");
                }
                else
                {
                    await ctx.RespondAsync($"<@{userID}> was not found. Register with a new allycode using ```;;r allycode```");
                }
                await m.DeleteAsync();
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
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
                bool found = false; String foundCode = "";
                GuildParse.Guild g = dh.getGuild(new uint[] { allycode }, helper);
                dh.logCommandInfo($"{ctx.Member} registering {allycode}");
                //if file exists, parse it
                users = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/users.txt"));
                dh.logCommandInfo($"Checking for existing registration");
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
                    dh.logCommandInfo($"{ctx.Member} already registered to {foundCode}");
                }
                else
                {
                    if (addUser(user, allycode, ctx.Member.Id.ToString(), g.guild[0].Id))
                    {
                        File.WriteAllText(commandFunctions.detectOS() + @"Data/users.txt", users.ToString());
                        await ctx.RespondAsync("<@" + ctx.Member.Id.ToString() + "> has  been registered to " + allycode + ". If you want to change your allycode, please unregister and register again with the new allycode.");
                        dh.logCommandInfo($"{ctx.Member} registered to c{allycode}");
                    }
                    else
                        await ctx.RespondAsync("<@" + ctx.Member.Id.ToString() + "> not registered to " + allycode + ". Check the allycode and try again.");
                }
                await m.DeleteAsync();
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
                    File.WriteAllText(commandFunctions.detectOS() + @"Data/users.txt", users.ToString());
                    await ctx.RespondAsync("<@" + ctx.Member.Id.ToString() + "> has  been registered to " + allycode + ". If you want to change your allycode, please unregister and register again with the new allycode.");
                }
                else
                    await ctx.RespondAsync("<@" + ctx.Member.Id.ToString() + "> not registered to " + allycode + ". Check the allycode and try again.");
            }
        }
        public bool addUser(JArray arr, uint allycode, string userID, string guildID)
        {
            if (commandFunctions.parseAllycode(allycode.ToString()) > 99999)
            {
                //Create new user
                JObject newUser = new JObject();
                newUser.Add("id", userID);
                newUser.Add("allycode", allycode);
                newUser.Add("guildID", guildID);
                //create default GA Toon list
                JArray GAToons = new JArray();
                string[] toons = { "GLREY", "SUPREMELEADERKYLOREN", "GRANDMASTERLUKE", "SITHPALPATINE", "DARTHMALAK", "GENERALSKYWALKER", "DARTHREVAN", "BASTILASHANDARK", "GRIEVOUS", "PADMEAMIDALA", "JEDIKNIGHTREVAN", "DARTHTRAYA", "ENFYSNEST" };
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
                string[] twtoons = { "GLREY", "SUPREMELEADERKYLOREN", "GRANDMASTERLUKE", "SITHPALPATINE", "GENERALSKYWALKER", "DARTHMALAK", "PADMEAMIDALA", "JEDIKNIGHTREVAN", "DARTHREVAN", "BASTILASHANDARK", "GRIEVOUS", "DARTHTRAYA", "BOSSK", "GEONOSIANBROODALPHA", "ENFYSNEST", "CAPITALNEGOTIATOR", "CAPITALMALEVOLENCE", "HOUNDSTOOTH", "MILLENNIUMFALCON" }; ;
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
            users = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/users.txt"));
            user = (JArray)users["users"];
            foreach (JObject obj in user.Children())
            {
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
            users = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/users.txt"));
            user = (JArray)users["users"];
            foreach (JObject obj in user.Children())
            {
                if (obj.Property("id").Value.ToString().Equals(userID))
                {
                    found = true;
                    Console.WriteLine("Found GUild ID");
                    return obj.Property("guildID").Value.ToString();
                }
            }
            return "";
        }
        #endregion
        #region GA Toon Add/Remove
        [Command("gaa"), Description("Add a toon for GA Compare")]
        public async Task gaAdd(CommandContext ctx, [RemainingText] string toon)
        {
            await toonAdd(ctx, toon, "GA");
        }
        [Command("gar"), Description("Remove a toon for GA Compare")]
        public async Task gaRemove(CommandContext ctx, [RemainingText] string toon)
        {
            await toonRemove(ctx, toon, "GA");
        }
        #endregion
        #region TW Toon Add/Remove
        [Command("twa"), Description("Add a toon for GA Compare")]
        public async Task twAdd(CommandContext ctx, [RemainingText] string toon)
        {
            await toonAdd(ctx, toon, "TW");
        }
        [Command("twr"), Description("Remove a toon for GA Compare")]
        public async Task twRemove(CommandContext ctx, [RemainingText] string toon)
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
                users = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/users.txt"));
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
                users = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/users.txt"));
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
                    File.WriteAllText(commandFunctions.detectOS() + @"Data/users.txt", users.ToString());
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
                users = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/users.txt"));
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
                    File.WriteAllText(commandFunctions.detectOS() + @"Data/users.txt", users.ToString());
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
        #region Toon Compare
        [Command("tc"), Description("Gets guild stats"), Hidden]
        public async Task toonCompare(CommandContext ctx, [Description("Ally Code to lookup")] string allycode, [Description("Toon to lookup in the guild")] string toon, string allycode2 = "")
        {
            string title = "", embeds = "";
            uint sd;
            uint parsedAllyCode, register;
            uint parsedAllyCode1 = 1, parsedAllyCode2 = 1;
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Toon Comparison",
                Color = new DiscordColor(0xFF0000) // red
            };
            helper = commandFunctions.login();
            if (allycode2.Equals(""))
            {
                parsedAllyCode2 = commandFunctions.parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
                parsedAllyCode1 = commandFunctions.checkAllycode(ctx, allycode);
            }
            else
            {
                parsedAllyCode1 = commandFunctions.checkAllycode(ctx, allycode);
                parsedAllyCode2 = commandFunctions.checkAllycode(ctx, allycode2);
            }
            try
            {
                if ((!(parsedAllyCode1 == 1) && !(parsedAllyCode2 == 1)))
                {
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                    DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
                    var interactivity = ctx.Client.GetInteractivityModule();
                    //  CharacterStrings d = new CharacterStrings();
                    String toonKey = "";
                    DataHelper dh = new DataHelper(ctx);
                    String defID = readToons(ctx, toon);
                    GAPlayer[] players = dh.getToonCompare(m, new uint[] { parsedAllyCode1, parsedAllyCode2 }, helper, defID);
                    string header = $"{players[0].name}  VS  {players[1].name}";
                    embed.ThumbnailUrl = $"https://swgoh.gg/game-asset/u/{defID}/";
                    title = $"={toon}=";
                    embed.Title = $"{toon} Comparison";
                    embeds = "```\n";
                    embeds += header + "\n";
                    embeds += dh.createTCLine("Health:", $"{players[0].toonCompare.finalHealth.ToString()}({players[0].toonCompare.modHealth.ToString()})", $"{players[1].toonCompare.finalHealth.ToString()}({players[1].toonCompare.modHealth.ToString()})");
                    embeds += dh.createTCLine("Protection:", $"{players[0].toonCompare.finalProt.ToString()}({players[0].toonCompare.modProtection.ToString()})", $"{players[1].toonCompare.finalProt.ToString()}({players[1].toonCompare.modProtection.ToString()})");
                    embeds += dh.createTCLine("Speed:", $"{players[0].toonCompare.finalSpeed.ToString()}({players[0].toonCompare.modSpeed.ToString()})", $"{players[1].toonCompare.finalSpeed.ToString()}({players[1].toonCompare.modSpeed.ToString()})");
                    embeds += dh.createTCLine("Crit Dam:", $"{(players[0].toonCompare.finalCritDam * 100).ToString()}({(players[0].toonCompare.modCritDamage * 100).ToString()})", $"{(players[1].toonCompare.finalCritDam * 100).ToString()}({(players[1].toonCompare.modCritDamage * 100).ToString()})");
                    embeds += dh.createTCLine("Potency:", $"{Math.Round((players[0].toonCompare.finalPot * 100), 2).ToString()}% ({Math.Round(players[0].toonCompare.modPotency * 100, 2).ToString()}%)", $"{Math.Round(players[1].toonCompare.finalPot * 100, 2).ToString()}% ({Math.Round(players[1].toonCompare.modPotency * 100, 2).ToString()}%)");
                    embeds += dh.createTCLine("Tenactiy:", $"{Math.Round((players[0].toonCompare.finalTen * 100), 2).ToString()}%({Math.Round(players[0].toonCompare.modTenacity * 100, 2).ToString()}%)", $"{Math.Round(players[1].toonCompare.finalTen * 100, 2).ToString()}%({Math.Round(players[1].toonCompare.modTenacity * 100, 2).ToString()}%)");
                    embeds += dh.createTCLine("Physical Damage:", $"{(players[0].toonCompare.finalPhysDam).ToString()}({players[0].toonCompare.modPhysicalDamage.ToString()})", $"{players[1].toonCompare.finalPhysDam.ToString()}({players[1].toonCompare.modPhysicalDamage.ToString()})");
                    embeds += dh.createTCLine("Special Damage:", $"{(players[0].toonCompare.finalSpecDam).ToString()}({players[0].toonCompare.modSpecialDamage.ToString()})", $"{players[1].toonCompare.finalSpecDam.ToString()}({players[1].toonCompare.modSpecialDamage.ToString()})");
                    embeds += dh.createTCLine("Phys Crit Chance:", $"{Math.Round((players[0].toonCompare.finalPhysCrit * 100), 2).ToString()}%({Math.Round(players[0].toonCompare.modPhysicalCriticalChance * 100, 2).ToString()}%)", $"{Math.Round(players[1].toonCompare.finalPhysCrit * 100, 2).ToString()}%({Math.Round(players[1].toonCompare.modPhysicalCriticalChance * 100, 2).ToString()}%)");
                    embeds += dh.createTCLine("Spec Crit Chance:", $"{Math.Round((players[0].toonCompare.finalSpecCrit * 100), 2).ToString()}%({Math.Round(players[0].toonCompare.modSpecialCriticalChance * 100, 2).ToString()}%)", $"{Math.Round(players[1].toonCompare.finalSpecCrit * 100, 2).ToString()}%({Math.Round(players[1].toonCompare.modSpecialCriticalChance * 100, 2).ToString()}%)");
                    embeds += dh.createTCLine("Armor:", $"{Math.Round((players[0].toonCompare.finalArmor * 100), 2).ToString()}%({Math.Round((players[0].toonCompare.modArmor * 100), 2).ToString()}%)", $"{Math.Round((players[1].toonCompare.finalArmor * 100), 2).ToString()}%({Math.Round((players[1].toonCompare.modArmor * 100), 2).ToString()}%)");
                    embeds += dh.createTCLine("Resistance:", $"{(Math.Round((players[0].toonCompare.finalResistance * 100), 2)).ToString()}%({Math.Round((players[0].toonCompare.modResistance * 100), 2).ToString()})", $"{Math.Round((players[1].toonCompare.finalResistance * 100), 2).ToString()}%({Math.Round((players[1].toonCompare.modResistance * 100), 2).ToString()}%)");
                    embeds += dh.createTCLine("Phys Crit Avoid:", $"{Math.Round((players[0].toonCompare.finalPhysCritAvoid * 100), 2).ToString()}%({Math.Round((players[0].toonCompare.modPhysCritAvoid * 100), 2).ToString()}%)", $"{Math.Round((players[1].toonCompare.finalPhysCritAvoid * 100), 2).ToString()}%({Math.Round((players[1].toonCompare.modPhysCritAvoid * 100), 2).ToString()}%)");
                    embeds += dh.createTCLine("# Zetas:", $"{players[0].toonCompare.zetas}", $"{players[1].toonCompare.zetas}");


                    embeds += "```";
                    embed.Description = embeds;
                    await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:", embed: embed);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n\n" + e.StackTrace);
            }
        }
        #endregion
        #region store Guild
        /*
        public async Task storeGuild(CommandContext ctx, uint allycode)
        {
            //Users.Users u = new Users.Users();
            JObject guilds;
            JArray guild;
            JArray roster;
            DataHelper dh = new DataHelper(ctx);
            try
            {
                string guildId = getGuildID(ctx.Member.Id.ToString());
                bool found = false;
                Console.WriteLine("Checking users...");
                guilds = JObject.Parse(File.ReadAllText((RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "" : $@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs") + @"\Data\guilds.txt"));
                guild = (JArray)guilds["guilds"];
                foreach (JObject obj in guild.Children())
                {
                    if (obj.Property("id").Value.ToString().Equals(guildId))
                    {
                        found = true;
                        if ((Convert.ToDateTime(obj.Property("lastUpdated").Value.ToString()) - DateTime.Now).Hours > 12)
                        {
                            GuildParse.Guild g = dh.getGuild(new uint[] { allycode }, helper);
                            JArray newRoster = new JArray();
                            foreach (GuildParse.Roster r in g.guild[0].Roster)
                            {
                                JObject member = new JObject();
                                member.Add("allycode", r.AllyCode);
                                member.Add("name", r.Name);
                                newRoster.Add(member);
                            }
                            obj["members"] = newRoster;
                        }
                        foreach (JObject sub in obj.Children())
                        {
                            if ((Convert.ToDateTime(sub.Property("lastUpdated").Value.ToString()) - DateTime.Now).Hours > 12)
                            {


                            }
                        }
                    }
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
                    File.WriteAllText((RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "" : $@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs") + @"\Data\guilds.txt", users.ToString());
                }
            }
        }
        */
        #endregion
        [Command("readlist"), Description("Get the URL to a guilds swgoh.gg profile")]
        public async Task readlist(CommandContext ctx)
        {
            JObject toons;
            toons = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data\toons.txt"));
            Console.WriteLine(Directory.GetCurrentDirectory());
            //readToons(ctx);
            //readToonsList(ctx);
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
                toons = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/defID.txt"));

                foreach (JObject j in toons.Children())
                {
                    Console.WriteLine(LevenshteinDistance.Compute(toon, j.Value<string>(), 50));
                }
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
                Console.WriteLine("Checking values");
                //if file exists, parse it
                toons = JObject.Parse(File.ReadAllText(commandFunctions.detectOS() + @"Data/toons.txt"));

                /*  List<TempObject> resultset = new List<TempObject>();
                  foreach (JProperty j in toons.Children())
                  {
                      try
                      {

                          int value = LevenshteinDistance.Compute(toon, j.Name.ToString(), 50);
                          TempObject o = new TempObject(j.Value.ToString(), value);
                          Console.WriteLine(value);
                          resultset.Add(o);
                      }
                      catch (Exception e)
                      {
                          Console.WriteLine(e.StackTrace + "\n\n" + e.Message);
                      }
                  }
                  //get the minimum number of modifications needed to arrive at the basestring
                  int minimumModifications = resultset.Min(c => c.place);
                  //gives you a list with all strings that need a minimum of modifications to become the
                  //same as the baseString
                  var closestlist = resultset.Where(c => c.place == minimumModifications);
                  Console.WriteLine("Min Mods: " + minimumModifications);
                  foreach (TempObject fruit in closestlist)
                  {
                      Console.WriteLine(fruit.defID);
                  }*/
                return toons.GetValue(toon, StringComparison.InvariantCultureIgnoreCase).ToString();

            }
            catch (Exception e) { Console.WriteLine(e.StackTrace + "\n\n" + e.Message); }
            return "";
        }
        public void readToonsList(CommandContext ctx, DiscordEmbedBuilder embed)
        {
            JObject toons;
            Database db = new Database();
            DataHelper dh = new DataHelper(ctx);
            CharacterDefID d = new CharacterDefID();
            String toonName = "";
            try
            {
                int j = 0;
                string embeds = "";
                foreach (KeyValuePair<string, string> entry in d.toons)
                {
                    embeds += entry.Value.ToString() + "\n";
                    if (j % 10 == 0 && j != 0)
                    {
                        embed.AddField($"==", embeds, true);
                        embeds = "";
                    }
                    j++;
                }
                // await ctx.RespondAsync("", embed: embed);
            }
            catch (Exception e) { Console.WriteLine(e.Message); }

        }

        public async Task gettest(CommandContext ctx, [Description("Ally Code to lookup")] uint allycode1)
        {
            helper = commandFunctions.login();
            DataHelper dh = new DataHelper(ctx);
            String data = helper.fetchPlayer(new uint[] { 729778685 });
            File.WriteAllText(commandFunctions.detectOS() + @"Data/test.txt", data.ToString());
            Console.WriteLine(helper.getStats(data));
            //await ctx.RespondAsync("https://swgoh.gg/g/" + p.Data.GuildId + "/" + p.Data.GuildName.Replace(" ", "-") + "/");
        }

        [Command("testnewApi"), Description("Get the URL to a guilds swgoh.gg profile")]
        public async Task getNewAPI(CommandContext ctx, [Description("Ally Code to lookup")] uint allycode1)
        {
            helper = commandFunctions.login();
            DataHelper dh = new DataHelper(ctx);
            String data = helper.fetchPlayer(new uint[] { 729778685 });
            File.WriteAllText(commandFunctions.detectOS() + @"Data/test.txt", data.ToString());
            Console.WriteLine(helper.getStats(data));
            //await ctx.RespondAsync("https://swgoh.gg/g/" + p.Data.GuildId + "/" + p.Data.GuildName.Replace(" ", "-") + "/");
        }
        //shittybot:YXBpLjE2MDExNDczMDkwMTMuYjI1c2VXMWhjblI1Y2c9PS5zd2dvaA===
        /*public uint parseAllycode(string allycode)
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
        public string detectOS()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return $@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\";
            }
            return "";
        }*/
        [Command("pe"), Description("Evaluate a player"), Aliases("")]
        public async Task evaluatePlayer(CommandContext ctx, [Description("Player to evaluate")] string allycode)
        {
            DiscordMessage m = await ctx.RespondAsync("processing request, standby...");
            uint parsedAllyCode1 = commandFunctions.checkAllycode(ctx, allycode);
            if (!(parsedAllyCode1 == 1))
            {
                helper = commandFunctions.login();
                DataHelper dh = new DataHelper(ctx);
                GAPlayer[] players = dh.getGAStats(m, new uint[] { parsedAllyCode1 }, helper, GAToonList);
                double score = calcModScore(players, ctx, m);
                calcGearScore(players, ctx, m);
                await m.ModifyAsync($"Mod score is: { score}");
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public double calcModScore(GAPlayer[] players, CommandContext ctx, DiscordMessage m)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Console.WriteLine(players[0].speedMods[1] + " : " + (players[0].toonGP * 1.0) / 100000);
            return Math.Round(((players[0].speedMods[1]) * 1.0) / ((players[0].toonGP * 1.0) / 100000), 2);
        }
        public double calcGearScore(GAPlayer[] players, CommandContext ctx, DiscordMessage m)
        {
            Console.WriteLine(Convert.ToDouble(players[0].G12 + players[0].G121 + players[0].G122 + players[0].G123 + players[0].G124 + players[0].G125) / (Convert.ToDouble(players[0].toonGP) / 100000));
            return 1.0;
        }
        /*public double calcG13Bonus(GAPlayer players)
        {
            return 1 + 2;
        }*/
    }
    /*
     
    Mod Quality: Number of +15 Speeds / (squad GP / 100000)
    Gear Quality: (Number of G12+ + G13 Bonus Score) / (Total GP / 100000)
    G13 Bonus score: 1 + (0.2 bonus per relic tier) (ex: r0 = 1, r1 = 1.2, ..., r7 = 2.4)
    Total Quality: Mod Quality + Gear Quality

     * */


    static class LevenshteinDistance
    {
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string source, string target, int threshold)
        {
            int length1 = source.Length;
            int length2 = target.Length;

            // Return trivial case - difference in string lengths exceeds threshhold
            if (Math.Abs(length1 - length2) > threshold) { return int.MaxValue; }

            // Ensure arrays [i] / length1 use shorter length 
            if (length1 > length2)
            {
                Swap(ref target, ref source);
                Swap(ref length1, ref length2);
            }

            int maxi = length1;
            int maxj = length2;

            int[] dCurrent = new int[maxi + 1];
            int[] dMinus1 = new int[maxi + 1];
            int[] dMinus2 = new int[maxi + 1];
            int[] dSwap;

            for (int i = 0; i <= maxi; i++) { dCurrent[i] = i; }

            int jm1 = 0, im1 = 0, im2 = -1;

            for (int j = 1; j <= maxj; j++)
            {

                // Rotate
                dSwap = dMinus2;
                dMinus2 = dMinus1;
                dMinus1 = dCurrent;
                dCurrent = dSwap;

                // Initialize
                int minDistance = int.MaxValue;
                dCurrent[0] = j;
                im1 = 0;
                im2 = -1;

                for (int i = 1; i <= maxi; i++)
                {

                    int cost = source[im1] == target[jm1] ? 0 : 1;

                    int del = dCurrent[im1] + 1;
                    int ins = dMinus1[i] + 1;
                    int sub = dMinus1[im1] + cost;

                    //Fastest execution for min value of 3 integers
                    int min = (del > ins) ? (ins > sub ? sub : ins) : (del > sub ? sub : del);

                    if (i > 1 && j > 1 && source[im2] == target[jm1] && source[im1] == target[j - 2])
                        min = Math.Min(min, dMinus2[im2] + cost);

                    dCurrent[i] = min;
                    if (min < minDistance) { minDistance = min; }
                    im1++;
                    im2++;
                }
                jm1++;
                if (minDistance > threshold) { return int.MaxValue; }
            }

            int result = dCurrent[maxi];
            return (result > threshold) ? int.MaxValue : result;
        }
        static void Swap<T>(ref T arg1, ref T arg2)
        {
            T temp = arg1;
            arg1 = arg2;
            arg2 = temp;
        }
    }
    public class TempObject
    {
        public string defID;
        public int place;
        public TempObject(string id, int placement)
        {
            defID = id;
            place = placement;
        }
    }
}