using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;

namespace SWGOH
{
    // note that in here we explicitly ask for duration. This is optional,
    // since we set the defaults.
    public class Commands
    {
        swgohHelpApiHelper helper;
        public SWGOHLegends legends { get; set; }
        [Command("reqs"), Description("Determines status of prerequesites for given legendary character")]
        public async Task getPrereqs(CommandContext ctx, [Description("Legendary/Event to check Character/Ship for minimum requirements")] String character, [Description("Ally Code to compare")] uint allycode)
        {
            // first retrieve the interactivity module from the client
            var interactivity = ctx.Client.GetInteractivityModule();

            await ctx.RespondAsync("Well Done");
        }
        [Command("greqs"), Description("Determines status of prerequesites for given legendary character")]
        public async Task getGuildPrereqs(CommandContext ctx, [Description("Legendary/Event to check Character/Ship for minimum requirements")] String character, [Description("Ally Code to compare")] uint allycode)
        {
            // first retrieve the interactivity module from the client
            var interactivity = ctx.Client.GetInteractivityModule();

            await ctx.RespondAsync("Well Done");
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
            helper.login();
        }

        public void test()
        {
            /* Console.WriteLine("Fetching Guild...");
             string guild = helper.fetchGuild(new uint[] { 729778685 });
             Console.WriteLine("Guild Retrieved...: ");
             guild = "{\"guild\":" + guild + "}";
             // Console.WriteLine(guild);
             // guild = guild.Remove(guild.Length - 1).Remove(0, 1);
             GuildParse.Guild gi = JsonConvert.DeserializeObject<GuildParse.Guild>(guild);
             uint[] members = new uint[gi.guild[0].Roster.Length];
             for (int i = 0; i < gi.guild[0].Roster.Length; i++)
             {
                 members[i] = gi.guild[0].Roster[i].AllyCode;
             }



             /*
              * This block tests fetching player(s)
              *
             Console.WriteLine("Fetching Player(s)...");
             string player = helper.fetchPlayer(new uint[] { 729778685, 155433894 });
             //Console.WriteLine(player);
             player = "{\"players\":" + player + "}";
             PlayerParse.Player gm = PlayerParse.Player.FromJson(player);
             Console.WriteLine("Player(s) retrieved...");
             Console.WriteLine(gm.PlayerList.Length + " Player(s) Retrieved...");
             PlayerParse.Roster[] toons = gm.PlayerList[0].Roster;
             foreach (PlayerParse.Roster toon in toons)
             {
                 Console.WriteLine(toon.NameKey);
             }



             /*
              * 
              * This block test pulling and processing a guild roster
              *
             Console.WriteLine("Fetching Guild...");
             string guild = helper.fetchGuild(new uint[] { 729778685 });
             Console.WriteLine("Guild Retrieved...: ");
             guild = "{\"guild\":" + guild + "}";
             Console.WriteLine(guild);
             // guild = guild.Remove(guild.Length - 1).Remove(0, 1);
             GuildParse.Guild gi = JsonConvert.DeserializeObject<GuildParse.Guild>(guild);
             uint[] members = new uint[gi.guild[0].Roster.Length];
             for (int i = 0; i < gi.guild[0].Roster.Length; i++)
             {
                 members[i] = gi.guild[0].Roster[i].AllyCode;
             }
             Console.WriteLine("Fetching Guild Members...");
             string guildroster = helper.fetchPlayer(members);
             Console.WriteLine("Guild Members Retrieved...");
             guildroster = "{\"players\":" + guildroster + "}";
             //guildroster = guildroster.Remove(guild.Length - 1).Remove(0, 1);
             PlayerParse.Player gm = PlayerParse.Player.FromJson(guildroster);
             Console.WriteLine("Guild Processed...");
             /**/
        }

        private struct ConfigJson
        {
            [JsonProperty("username")]
            public string username { get; private set; }

            [JsonProperty("password")]
            public string password { get; private set; }
        }

    }
}
