using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using SWGOH_Prereqs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SWGOH
{
    class TestCommands
    {
        CommandFunctions commandFunctions = new CommandFunctions();
        public swgohHelpApiHelper helper;

        [Command("twsub"), Description("Gets guild overview")]
        public async Task getOverview(CommandContext ctx, string allycode1, string allycode2)
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
                Stopwatch stop = new Stopwatch();

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
                            guilds = dh.getGuild(new uint[] { parsedAllyCode1, parsedAllyCode2 }, helper);
                            await m.ModifyAsync($"\n\n....Data retrieved.\n\n");
                            break;
                        }
                        catch { tries++; }
                    }
                    if (guilds != null)
                    {
                        if (guilds.guild.Length > 1)
                        {
                            long presetSum = 0;
                            List<uint> codes = new List<uint>() { 746381165, 876947886, 155433894, 237231378, 739661914, 729778685, 248266291, 272529563, 826671465, 279676631, 145914831, 163932394, 633275132, 412342476, 295692547, 437671438, 836422174, 824746243, 925653874, 922689332, 526651765, 937215723 };
                            List<GuildParse.Roster> test = new List<GuildParse.Roster>();
                            GuildParse.Roster[] shortRost = new GuildParse.Roster[guilds.guild[0].Roster.Length - codes.Count];
                            for (int j = 0; j < guilds.guild[0].Roster.Length; j++)
                            {
                                if (!codes.Contains(guilds.guild[0].Roster[j].AllyCode))
                                {
                                    test.Add(guilds.guild[0].Roster[j]);
                                }
                                else
                                {
                                    presetSum += guilds.guild[0].Roster[j].Gp;
                                }
                            }
                            for (int i = 0; i < test.Count; i++)
                            {
                                shortRost[i] = test[i];
                            }
                            stop.Reset();
                            stop.Start();
                            Console.WriteLine("doing math");
                            Console.WriteLine(commandFunctions.backtrack(0, presetSum, 200000000, shortRost));
                            //Console.WriteLine(commandFunctions.backtrack(0, 0, 200000000, guilds.guild[0].Roster));
                            stop.Stop();
                            Console.WriteLine(stop.Elapsed.ToString("G"));
                        }
                    }
                }
            }
        }
    }
}