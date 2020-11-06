using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SWGOH;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SWGOH_Prereqs
{
    class DataHelper
    {
        CommandContext ctx;
        public DataHelper(CommandContext ctxt) { ctx = ctxt; }
        public bool checkCommand(string command, IReadOnlyDictionary<string, Command> commands)
        {
            foreach (string s in commands.Keys) { if (command.ToLower().Trim().Equals(s)) { return true; } }
            return false;
        }
        public void SortMembers(List<GuildMember> gm, int sort, string sortType)
        {
            if (sortType.Equals("RELICS")) { gm.Sort((x, y) => x.relics.CompareTo(y.relics)); }
            if (sortType.Equals("GP")) { gm.Sort((x, y) => x.getGP().CompareTo(y.getGP())); }
            if (sortType.Equals("NAME")) { gm.Sort((x, y) => x.getMemberName().CompareTo(y.getMemberName())); }
            if (sort == 1) { gm.Reverse(); }
            logCommandInfo("Sorting by: " + sortType);
        }
        public string buildPlayerStats(PlayerParse.Player r, DiscordEmbedBuilder b, DiscordEmbedBuilder b2, string sort, string ID)
        {
            Database db = new Database();
            string sortedBy = "";
            int i = 0;
            List<GuildMember> gm = new List<GuildMember>(r.PlayerList.Length);
            logCommandInfo($"Building Members For Guild Stats");
            foreach (PlayerParse.PlayerElement pe in r.PlayerList)
            {
                GuildMember g = new GuildMember(pe);
                g.calcstats();
                gm.Add(g);
                Hashtable hash = new Hashtable();
                hash.Add("GuildID", ID);
                hash.Add("Allycode", pe.AllyCode);
                // db.InsertRow(hash, "GuildMembers");
            }
            try
            {
                switch (sort.ToUpper())
                {
                    case "GP":
                    case "GP ASC":
                    case "GP ASCENDING":
                        SortMembers(gm, 0, "GP");
                        sortedBy = "GP ASCENDING";
                        break;
                    case "NAME":
                    case "NAME ASC":
                    case "NAME ASCENDING":
                        SortMembers(gm, 0, "NAME");
                        sortedBy = "NAME ASCENDING";
                        break;
                    case "NAME DESC":
                    case "NAME DESCENDING":
                        SortMembers(gm, 1, "NAME");
                        sortedBy = "NAME DESCENDING";
                        break;
                    case "RELICS":
                    case "RELICS DESC":
                    case "RELICS DESCENDING":
                        SortMembers(gm, 1, "RELICS");
                        sortedBy = "RELICS DESCENDING";
                        break;
                    case "RELICS ASC":
                    case "RELICS ASCENDING":
                        SortMembers(gm, 0, "RELICS");
                        sortedBy = "RELICS ASCENDING";
                        break;
                    default:
                    case "GP DESC":
                    case "GP DESCENDING":
                        SortMembers(gm, 1, "GP");
                        sortedBy = "GP DESCENDING";
                        break;
                }
            }
            catch { SortMembers(gm, 1, "GP"); sortedBy = "GP DESCENDING:"; }
            foreach (GuildMember gmb in gm)
            {
                if (i < 25) { b.AddField($"{gmb.getMemberName()}=", gmb.buildEmbedField().Replace(",", "."), true); }
                else { b2.AddField($"={gmb.getMemberName()}=", gmb.buildEmbedField().Replace(",", "."), true); }
                i++;
            }
            return sortedBy;
        }
        public TWGuild newGuildStats(PlayerParse.Player guild, string[] toons)
        {
            String guildID = guild.PlayerList[0].GuildRefId.ToString();
            string ignoreList = getIgnoreList(guildID);
            string[] ignore;
            List<string> ignored = null;
            if (ignoreList.Length > 0)
            {

                ignoreList = ignoreList.Substring(0, ignoreList.LastIndexOf(','));
                ignore = ignoreList.Split(',');
                ignored = new List<string>(ignore.Length);
                Console.WriteLine("Ignore List returned " + ignore.Length);
            }
            else { ignore = null; }
            logCommandInfo($"Fetching {guild.PlayerList[0].GuildName} Stats for TW");
            Database db = new Database();
            TWGuild guildObj = new TWGuild();
            bool ignoreMe = false;
            try
            {
                if (ignored != null)
                {
                    foreach (String s in ignore)
                    {
                        ignored.Add(s);
                        Console.WriteLine(s + " added to guild Obj");
                    }
                    guildObj.ignoreList = ignored;
                    Console.WriteLine("ignore list added to guild obj");
                }
                List<Toons> toonList = new List<Toons>(toons.Length);
                foreach (string s in toons)
                {
                    Toons temp = new Toons(s, 6);
                    toonList.Add(temp);
                }
                int[] speeds = new int[4];
                foreach (PlayerParse.PlayerElement pe in guild.PlayerList)
                {
                    if (ignore != null)
                    {
                        foreach (string s in ignore)
                        {
                            if (pe.AllyCode.ToString().Equals(s))
                            {
                                Console.WriteLine($"\n\n{pe.AllyCode} ignored\n\n");
                                ignoreMe = true;
                                break;
                            }
                            else { ignoreMe = false; }
                        }
                        if (!ignoreMe)
                        {
                            Console.WriteLine("Building for " + pe.AllyCode);
                            buildTWInfo(guildObj, toons, pe, toonList, speeds);
                            ignoreMe = false;
                        }
                    }
                    else { buildTWInfo(guildObj, toons, pe, toonList, speeds); }

                }
                guildObj.speedMods = speeds;
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
            return guildObj;
        }
        public void buildTWInfo(TWGuild guildObj, string[] toons, PlayerParse.PlayerElement pe, List<Toons> toonList, int[] speeds)
        {
            foreach (PlayerParse.Roster r in pe.Roster)
            {
                int zetas = 0;
                if (Array.IndexOf(toons, r.DefId) >= 0)
                {
                    Toons tempToon = toonList.Find(x => x.id.Equals(r.DefId));
                    if (r.CombatType == 2)
                    {
                        //is ship
                        if (r.Rarity >= 5) { tempToon.stars[r.Rarity - 5]++; }
                        tempToon.Total++;
                        tempToon.ship = true;
                    }
                    else
                    {
                        //is toon
                        if (r.Rarity >= 5) { tempToon.stars[r.Rarity - 5]++; }
                        if (r.Gear >= 11) { tempToon.gear[r.Gear - 11]++; }
                        tempToon.Total++;
                        if (r.Relic.CurrentTier > 1) { tempToon.relics[r.Relic.CurrentTier - 2]++; tempToon.totRel++; }
                        if (r.Gp >= 16000)
                        {
                            if (r.Gp >= 20000) tempToon.gp20++;
                            else tempToon.gp16++;
                        }
                        foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { if (skill.Tier == skill.Tiers) { zetas++; guildObj.zetas++; } } }
                        if (zetas == 1) tempToon.numZetas[0]++;
                        if (zetas == 2) tempToon.numZetas[1]++;
                        if (zetas == 3) tempToon.numZetas[2]++;
                        if (zetas == 4) tempToon.numZetas[3]++;
                        if (zetas == 5) tempToon.numZetas[4]++;
                        if (zetas == 6) tempToon.numZetas[5]++;
                    }
                }
                guildObj.toonList = toonList;
                foreach (PlayerParse.Mod m in r.Mods)
                {
                    foreach (PlayerParse.SecondaryStat s in m.SecondaryStat)
                    {
                        if (s.UnitStat == 5) { checkModSpeed(s.Value, speeds); }
                        if (s.UnitStat == 41) { if (s.Value >= 100) { guildObj.off100++; } }
                    }
                    if (m.Pips == 6) { guildObj.sixStarMods++; }
                }
                switch (r.Gear)
                {
                    case 11:
                        guildObj.G11++;
                        break;
                    case 12:
                        switch (r.Equipped.Length)
                        {
                            case 0:
                                guildObj.G12++;
                                break;
                            case 1:
                                guildObj.G121++;
                                break;
                            case 2:
                                guildObj.G122++;
                                break;
                            case 3:
                                guildObj.G123++;
                                break;
                            case 4:
                                guildObj.G124++;
                                break;
                            case 5:
                                guildObj.G125++;
                                break;
                        }
                        break;
                    case 13:
                        guildObj.G13++;
                        break;
                }
                if (r.Relic != null) { if (r.Relic.CurrentTier - 2 >= 0) { guildObj.TotalRelics++; guildObj.relics[r.Relic.CurrentTier - 2]++; } }
            }
        }
        public double buildCharGP(GuildParse.Roster[] r, string toonType)
        {
            logCommandInfo($"Building GP Values for TW");
            double GP = 0;
            if (toonType.Equals("Char")) { foreach (GuildParse.Roster rost in r) { GP += rost.GpChar; } }
            if (toonType.Equals("Fleet")) { foreach (GuildParse.Roster rost in r) { GP += rost.GpShip; } }
            return GP;
        }
        #region helper functions
        public string createLineDocs(string category, string left)
        {
            if (category.Length > 0) { return "" + category + addPadding(9 - category.Length) + " :: " + addPadding(4 - left.Length) + left + addPadding(2) + "\n"; }
            else { return category + left + addPadding(2) + "\n"; }
        }
        public string createLine(string category, string left)
        {
            if (category.Length > 0) { return category + addDots(15 - category.Length) + addDots(6 - left.Length) + left + addPadding(2) + "\n"; }
            else { return category + left + addPadding(2) + "\n"; }
        }
        public string createHeaderLine(string category, string left, string right)
        {
            return category + addDots(13 - category.Length) + addDots(10 - left.Length) + left + addPadding(2) + "::" + addPadding(2) + right + "\n";
        }
        public string createLine(string category, string left, string right)
        {
            return category + addDots(13 - category.Length - left.Length) + left + "::" + right + "\n";
        }
        public string createShortLine(string category, string left, string right)
        {
            return category + addDots(11 - category.Length - left.Length) + left + "::" + right + "\n";
        }
        public string createShortLine(string category, string left)
        {
            return category + addDots(11 - category.Length - left.Length) + left + "\n";
        }
        public string createLongLine(string category, string left, string right)
        {
            return category + addDots(20 - category.Length - left.Length) + left + addPadding(2) + "::" + addPadding(2) + right + "\n";
        }
        public string createTCLine(string category, string left, string right)
        {
            return category + addDots(30 - category.Length - left.Length) + left + "::" + right + "\n";
        }
        public string addPadding(int space)
        {
            string s = "";
            for (int i = 0; i < space; i++) { s += " "; }
            return s;
        }
        public string addDots(int space)
        {
            string s = "";
            for (int i = 0; i < space; i++) { s += "."; }
            return s;
        }
        #endregion
        public GAPlayer[] getGAStats(DiscordMessage me, uint[] allycodes, swgohHelpApiHelper helper, string[] list)
        {
            CharacterDefID d = new CharacterDefID();
            GAPlayer[] toons = new GAPlayer[2];
            GAPlayer player1 = new GAPlayer();
            GAPlayer player2 = new GAPlayer();
            toons[0] = player1;
            toons[1] = player2;
            me.RespondAsync("Retrieving players....");
            string statRoster = helper.getStats(getGuildMemberString(allycodes, helper));
            PlayerParse.Player player = JsonConvert.DeserializeObject<PlayerParse.Player>("{\"players\":" + statRoster + "}");
            //File.WriteAllText(@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\players.json", statRoster);
            try
            {
                me.ModifyAsync(me.Content + $"\n \nBuilding stats for {player.PlayerList[0].Name}");
                getRoster(player.PlayerList[0], player1, list);
                me.ModifyAsync(me.Content + $"\n\nBuilding stats for {player.PlayerList[0].Name}");
                getRoster(player.PlayerList[1], player2, list);
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
            logCommandInfo($"Building GA Stats");
            for (int i = 0; i < player.PlayerList.Length; i++)
            {
                toons[i].AllyCode = player.PlayerList[i].AllyCode;
                toons[i].name = player.PlayerList[i].Name;
                toons[i].totGP = (int)player.PlayerList[i].Stats[0].Value;
                toons[i].shipGP = (int)player.PlayerList[i].Stats[2].Value;
                toons[i].toonGP = (int)player.PlayerList[i].Stats[1].Value;
                toons[i].offWon = (int)player.PlayerList[i].Stats[13].Value;
                toons[i].defend = (int)player.PlayerList[i].Stats[14].Value;
                toons[i].under = (int)player.PlayerList[i].Stats[17].Value;
                toons[i].fullClear = (int)player.PlayerList[i].Stats[16].Value;
                toons[i].Banners = (int)player.PlayerList[i].Stats[15].Value;
                int[] speeds = new int[4];

                foreach (PlayerParse.Roster r in player.PlayerList[i].Roster)
                {
                    if (r.CombatType == 2)
                    {
                        //is ship
                    }
                    else
                    {
                        foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { if (skill.Tier == 8) { toons[i].Zetas++; } } }
                        if (r.Relic != null) { if (r.Relic.CurrentTier - 2 >= 0) { toons[i].totalRelics++; toons[i].relics[r.Relic.CurrentTier - 2]++; } }
                        foreach (PlayerParse.Mod m in r.Mods)
                        {
                            foreach (PlayerParse.SecondaryStat s in m.SecondaryStat)
                            {
                                if (s.UnitStat == 5) { checkModSpeed(s.Value, speeds); }
                                if (s.UnitStat == 41) { if (s.Value >= 100) { toons[i].off100++; } }
                            }
                            if (m.Pips == 6) { toons[i].sixStarMods++; }
                        }
                        switch (r.Gear)
                        {
                            case 11:
                                toons[i].G11++;
                                break;
                            case 12:
                                switch (r.Equipped.Length)
                                {
                                    case 0:
                                        toons[i].G12++;
                                        break;
                                    case 1:
                                        toons[i].G121++;
                                        break;
                                    case 2:
                                        toons[i].G122++;
                                        break;
                                    case 3:
                                        toons[i].G123++;
                                        break;
                                    case 4:
                                        toons[i].G124++;
                                        break;
                                    case 5:
                                        toons[i].G125++;
                                        break;
                                }
                                break;
                            case 13:
                                toons[i].G13++;
                                break;
                        }
                    }

                }
                toons[i].speedMods = speeds;
            }
            return toons;
        }
        public void getRoster(PlayerParse.PlayerElement pe, GAPlayer gap, string[] list)
        {
            logCommandInfo($"Fetching Roster Stats for {pe.Name}");
            CharacterDefID d = new CharacterDefID();
            foreach (PlayerParse.Roster r in pe.Roster)
            {
                Console.WriteLine("Comparing " + r.DefId);
                if (Array.IndexOf(list, r.DefId) >= 0)
                {
                    GAToon toon = new GAToon(r.DefId);
                    toon.starLevel = (int)r.Rarity;
                    toon.name = d.toons[r.DefId];
                    toon.GP = (int)r.Gp;
                    toon.speed = (int)r.Stats.Final["Speed"];
                    toon.physDam = (int)r.Stats.Final["Physical Damage"];
                    if (r.CombatType == 2)
                    {
                        //is ship
                        toon.isShip = true;
                    }
                    else
                    {
                        toon.specDam = (int)r.Stats.Final["Special Damage"];
                        toon.gearLevel = (int)r.Gear;
                        toon.gearEquipped = (int)r.Equipped.Length;
                        if (r.Relic.CurrentTier > 1) { toon.relic = (int)(r.Relic.CurrentTier - 2); }
                        foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { if (skill.Tier == 8) { toon.Zetas++; } } }
                    }
                    Console.WriteLine("Adding " + r.DefId);
                    gap.toonsOwned.Add(toon);
                }
            }
        }
        public string getGuildMemberString(uint[] ac, swgohHelpApiHelper helper)
        {
            dynamic obj = new ExpandoObject();
            obj.allyCode = 1;
            obj.name = 1;
            obj.roster = 1;
            obj.stats = 1;
            string guild = helper.fetchPlayer(ac, null, null, obj);

            return guild;
        }
        public Tuple<string, PlayerParse.Player> getGuildMembersRecursiveStart(GuildParse.Roster[] r, swgohHelpApiHelper helper, int size)
        {
            logCommandInfo("Made it to recursive call");

            uint[] members = new uint[r.Length];
            for (int i = 0; i < r.Length; i++) { members[i] = r[i].AllyCode; }
            return startRecursion(members, helper, size);
        }
        public Tuple<string, PlayerParse.Player> getGuildMembersRecursiveStart(uint[] r, swgohHelpApiHelper helper, int size)
        {
            logCommandInfo("Made it to recursive call");
            return startRecursion(r, helper, size);
        }
        public string getGuildMembersRecursive(uint[] allycodes, swgohHelpApiHelper helper, int size)
        {
            string members = "", allies = "";
            if (allycodes.Length > size)
            {
                uint[] getmembers = new List<uint>(allycodes).GetRange(0, size).ToArray();
                for (int i = 0; i < getmembers.Length; i++) { allies += getmembers[i] + ":"; }
                //     logCommandInfo(allies);
                uint[] newMembers = new List<uint>(allycodes).GetRange(size, allycodes.Length - size).ToArray();
                //    logCommandInfo($"Sending out {getmembers.Length} Allycodes::::Passing {newMembers.Length} to next recursive call.");
                members += getGuildMembersRecursive(getmembers, helper, size);
                return members.Remove(members.LastIndexOf("]")) + "," + getGuildMembersRecursive(newMembers, helper, size).Substring(1);
            }
            else { logCommandInfo("Last Recursive call for this guild"); }
            members += helper.fetchPlayer(allycodes);

            return members;
        }
        public GuildParse.Guild getGuild(uint[] ac, swgohHelpApiHelper helper)
        {
            dynamic obj = new ExpandoObject();
            obj.name = 1;
            obj.roster = 1;
            obj.stats = 1;
            string guild = "";
            guild = helper.fetchGuild(ac);
            guild = "{\"guild\":" + guild + "}";

            File.WriteAllText("test.json", guild);
            GuildParse.Guild gi = JsonConvert.DeserializeObject<GuildParse.Guild>(guild);
            return gi;

        }
        public string getGuildString(uint[] ac, swgohHelpApiHelper helper)
        {
            dynamic obj = new ExpandoObject();
            obj.name = 1;
            obj.roster = 1;
            obj.stats = 1;
            string guild = "";
            guild = helper.fetchGuild(ac);
            // guild = "{\"guild\":" + guild + "}";

            File.WriteAllText("test.json", guild);
            return guild;

        }
        public void checkModSpeed(double value, int[] speeds)
        {
            if (value >= 10)
            {
                if (value >= 15)
                {
                    if (value >= 20)
                    {
                        if (value >= 25) { speeds[3]++; }
                        speeds[2]++;
                    }
                    speeds[1]++;
                }
                speeds[0]++;
            }
        }
        public void logCommandInformation(String result)
        {
            String message = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " ::  " }User: {ctx.Member.DisplayName} executed command {ctx.Command}. Command was {result}.  Args:  {ctx.RawArgumentString}\n";
            if (ctx.Guild is null)
                File.AppendAllText(detectOS() + $@"Logs/{ctx.User}.txt", message);
            else
                File.AppendAllText(detectOS() + $@"Logs/{ctx.Guild.Name}.txt", message);
        }
        public void logCommandInfo(String info)
        {
            if (ctx.Guild is null)
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    File.AppendAllText(detectOS() + $@"Logs\{ctx.User}.txt", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " :: " + info + " \n");
                else
                    File.AppendAllText(detectOS() + $@"Logs/{ctx.User}.txt", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " :: " + info + " \n");
            else
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    File.AppendAllText(detectOS() + $@"Logs\{ctx.Guild.Name}.txt", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " :: " + info + " \n");
                else
                    File.AppendAllText(detectOS() + $@"Logs/{ctx.Guild.Name}.txt", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " :: " + info + " \n");
            }
        }
        public uint[] compareMemberList(PlayerParse.Player players1, GuildParse.Roster[] r)
        {
            uint[] foundCodes = new uint[players1.PlayerList.Length];
            uint[] neededCodes = new uint[r.Length - players1.PlayerList.Length];
            uint[] guildCodes = new uint[r.Length];
            logCommandInfo("Guild Size: " + r.Length + ":: Current Roster Size: " + players1.PlayerList.Length + ":: Needed Members: " + neededCodes.Length);
            for (int i = 0; i < players1.PlayerList.Length; i++) { foundCodes[i] = Convert.ToUInt32(players1.PlayerList[i].AllyCode); }
            for (int i = 0; i < r.Length; i++) { guildCodes[i] = Convert.ToUInt32(r[i].AllyCode); }
            IEnumerable<uint> aOnlyNumbers = guildCodes.Except(foundCodes);
            int f = 0;
            foreach (uint a in aOnlyNumbers) { neededCodes[f] = a; f++; }
            return neededCodes;
        }
        public PlayerParse.Player getInformation(DiscordMessage m, GuildParse.GuildMember gm, swgohHelpApiHelper helper, int startSize)
        {
            /*
             * Let's start by trying to get our members in 2 chunks
             */
            try
            {

                m.ModifyAsync(m.Content + $"\n\nRetrieving {gm.Name} guild data");
                Tuple<string, PlayerParse.Player> t = getGuildMembersRecursiveStart(gm.Roster, helper, startSize);
                /*
                 *Pull out the player object and get player counts 
                 */
                PlayerParse.Player players1 = t.Item2;
                int receivedCount = t.Item2.PlayerList.Length;
                int guildMemberCount = gm.Roster.Length;
                /*
                 * Save the JSON string returned
                 */
                string guildString = t.Item1;
                /*
                 * Let's do some logging just in case
                 */
                logCommandInfo($"Guild Name::{gm.Name} Member Count: " + guildMemberCount);
                logCommandInfo("Received Count: " + receivedCount);
                /************************************************************************************
                 * While the players we pull from the API is less than the expected number of players,
                 * let's keep trying to fill out the roster.
                 ********************************************************************/
                m.ModifyAsync($"\n\nRetrieving {gm.Name}: have {receivedCount}/{guildMemberCount}");
                while (guildMemberCount != receivedCount)
                {
                    /*
                     * Get the allycodes that we haven't retrieved
                     */
                    uint[] members = compareMemberList(players1, gm.Roster);
                    logCommandInfo("Remaining: " + members.Length);
                    foreach (uint u in members) { Console.WriteLine("AllyCode: " + u); }
                    /**********************************************************************************
                     * Try to get the remaining allycodes in chunks of 10. This may slow down the bot 
                     * in low traffic times but should be safer in high traffic times
                     *********************************************************************************/
                    if (startSize == 25)
                    {
                        t = getGuildMembersRecursiveStart(members, helper, 10);
                    }
                    else
                    {
                        t = getGuildMembersRecursiveStart(members, helper, 5);
                    }
                    /*****************************************************************
                     * Get the new JSON and combine it with the JSON we already have 
                     * and see how many allycodes we have now
                     ****************************************************************/
                    string newmember = t.Item1;
                    guildString = guildString.Remove(guildString.LastIndexOf("]")) + "," + newmember.Substring(1);
                    players1 = JsonConvert.DeserializeObject<PlayerParse.Player>("{\"players\":" + guildString + "}");
                    receivedCount = players1.PlayerList.Length;

                    logCommandInfo($"Guild Name::{gm.Name} Member Count: " + guildMemberCount);
                    logCommandInfo("Received Count: " + receivedCount);
                    m.ModifyAsync($"\n\nRetrieving {gm.Name}: have {receivedCount}/{guildMemberCount}");
                }
                foreach (PlayerParse.PlayerElement pe in players1.PlayerList)
                {
                    Console.WriteLine(pe.AllyCode + " : " + pe.Name);
                }
                m.ModifyAsync(m.Content + "\n\nCalling Crinolo to Get mod stats");
                int tries = 0;
                if (players1.PlayerList.Length == gm.Roster.Length)
                {
                    // File.WriteAllText(@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\players.json", guildString);
                    while (tries < 5)
                    {
                        try
                        {
                            m.ModifyAsync(m.Content + $"\n\nCrinolo try {tries + 1}/5");
                            string statRoster = helper.getStats(guildString);
                            players1 = JsonConvert.DeserializeObject<PlayerParse.Player>("{\"players\":" + statRoster + "}");
                            return players1;

                        }
                        catch (Exception e) { tries++; Console.WriteLine(e.StackTrace); Console.WriteLine(e.Message); }
                    }
                    return players1;
                }
                return null;
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); return null; }

        }
        public Tuple<string, PlayerParse.Player> startRecursion(uint[] allycodes, swgohHelpApiHelper helper, int size)
        {
            string guild = "", originalGuild = "";
            dynamic obj = new ExpandoObject();
            obj.name = 1;
            obj.roster = 1;
            obj.stats = 1;
            obj.allyCode = 1;
            logCommandInfo(allycodes.Length.ToString());
            guild = getGuildMembersRecursive(allycodes, helper, size);
            originalGuild = guild;
            guild = "{\"players\":" + guild + "}";
            //File.WriteAllText(@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\players.json", guild);
            PlayerParse.Player player = JsonConvert.DeserializeObject<PlayerParse.Player>(guild);
            return Tuple.Create(originalGuild, player);
        }
        public void guildChar(PlayerParse.Player player, CommandContext ctx, DiscordEmbedBuilder embed, string toon)
        {
            embed.ThumbnailUrl = $"https://swgoh.gg/game-asset/u/{toon}/";
            Console.WriteLine("Building");
            GuildToon gt = new GuildToon()
            {
                members = player.PlayerList.Length
            };
            string[] playerStrings = new string[5];
            int strings = 0;
            Console.WriteLine("Iterating members");
            foreach (PlayerParse.PlayerElement pe in player.PlayerList)
            {
                string playerString = "";
                Console.WriteLine($"Iterating Rosters: {pe.Name}");
                foreach (PlayerParse.Roster r in pe.Roster)
                {
                    int zetas = 0;
                    if (r.DefId.Equals(toon))
                    {
                        Console.WriteLine("Toon found");
                        if (r.CombatType == 2) { gt.isShip = true; Console.WriteLine("Ship"); }
                        playerString = $"{r.Rarity}|{String.Format("{0,2:00}", r.Gear)}|{String.Format("{0,2:00}", ((r.Gear == 13) ? (r.Relic.CurrentTier - 2) : (r.Equipped.Length)))}|{String.Format("{0,3:000}", r.Stats.Final["Speed"])}|{String.Format("{0,4:0000}", r.Stats.Final["Physical Damage"])}|{pe.Name}";
                        gt.totalCount++;
                        Console.WriteLine($"{gt.totalCount} {toon}s");
                        PlayerToon pt = new PlayerToon()
                        {
                            name = pe.Name,
                            star = r.Rarity,
                            gearLevel = r.Gear,
                            gearEquipped = r.Equipped.Length,
                            GP = (int)r.Gp,
                            level = (int)r.Level
                        };
                        Console.WriteLine("Calcing rarity");
                        if (r.Rarity > 4) { gt.stars[r.Rarity - 5]++; }
                        Console.WriteLine("Calcing skills");
                        foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { if (skill.Tier == 8) { zetas++; } } }
                        pt.zetas = zetas;
                        Console.WriteLine("Calcing zetas");
                        if (zetas > 0)
                            gt.zetas[zetas - 1]++;
                        Console.WriteLine("Calcing relics");
                        if (r.Relic != null) { if (r.Relic.CurrentTier - 2 >= 0) { gt.relics[r.Relic.CurrentTier - 2]++; pt.relic = r.Relic.CurrentTier - 2; } }
                        pt.speed = r.Stats.Final["Speed"];
                        pt.offense = r.Stats.Final["Physical Damage"];
                        Console.WriteLine("Calcing gear");
                        switch (r.Gear)
                        {
                            case 10:
                                gt.gearLevel[0]++;
                                break;
                            case 11:
                                gt.gearLevel[1]++;
                                break;
                            case 12:
                                switch (r.Equipped.Length)
                                {
                                    case 0:
                                        gt.gearLevel[2]++;
                                        break;
                                    case 1:
                                        gt.gearLevel[3]++;
                                        break;
                                    case 2:
                                        gt.gearLevel[4]++;
                                        break;
                                    case 3:
                                        gt.gearLevel[5]++;
                                        break;
                                    case 4:
                                        gt.gearLevel[6]++;
                                        break;
                                    case 5:
                                        gt.gearLevel[7]++;
                                        break;
                                }
                                break;
                            case 13:
                                gt.gearLevel[8]++;
                                break;
                        }
                        Console.WriteLine("Adding to list");
                        gt.players.Add(pt);
                    }
                }

                if (playerString.Length == 0)
                {
                    PlayerToon pt = new PlayerToon()
                    {
                        name = pe.Name,
                        star = 0,
                        gearLevel = 0,
                        gearEquipped = 0,
                        speed = 0,
                        offense = 0
                    };
                    gt.players.Add(pt);
                }
            }
            Console.WriteLine("Done finding toons");
            Console.WriteLine(gt.players.Count + "members");
            var ordered = gt.players.OrderByDescending(x => x.star).ThenByDescending(y => y.gearLevel).ThenByDescending(z => z.relic).ThenByDescending(a => a.gearEquipped).ThenByDescending(b => b.speed);
            String playerStringer = "";
            int i = 0;
            try
            {
                foreach (var p in ordered)
                {
                    Console.WriteLine(i);
                    if (i % 10 == 0 && i != 0)
                    {

                        int test = (i / 10) - 1;
                        Console.WriteLine(test);
                        playerStrings[test] = playerStringer;
                        playerStringer = "";
                    }
                    if (!gt.isShip)
                        playerStringer += $"{p.star}|{String.Format("{0,2:0}", p.gearLevel)}|{String.Format("{0,2:0}", ((p.gearLevel == 13) ? (p.relic) : (p.gearEquipped)))}|{String.Format("{0,3:0}", p.speed)}|{String.Format("{0,4:0}", p.offense)}|{p.name}\n";
                    else
                        playerStringer += $"{p.star}|{String.Format("{0,3:0}", p.level)}|{String.Format("{0,6:0}", p.GP)}|{String.Format("{0,3:0}", p.speed)}|{String.Format("{0,4:0}", p.offense)}|{p.name}\n";
                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            Console.WriteLine("Built strings");
            int index = i / 10;
            if (i % 10 == 0)
                playerStrings[index - 1] = playerStringer;
            else
                playerStrings[index] = playerStringer;
            strings++;

            string embeds = "", title = "";
            #region summary
            title = "==Summary==\n";
            embeds = "```CSS\n";
            embeds += createLine("Total:", $"{gt.totalCount}/{gt.members}");
            embeds += createLine("7★:", $"{gt.stars[2]}");
            embeds += createLine("6★:", $"{gt.stars[1]}");
            embeds += createLine("5★:", $"{gt.stars[0]}");
            if (!gt.isShip)
            {
                embeds += createLine("1Z:", gt.zetas[0].ToString());
                if (!toon.Equals("BASTILASHANDARK") && !toon.Equals("ENFYSNEST"))
                {
                    embeds += createLine("2Z:", gt.zetas[1].ToString());
                    if (toon.Equals("GLREY") || toon.Equals("SUPREMELEADERKYLOREN") || toon.Equals("GENERALSKYWALKER") || toon.Equals("DARTHREVAN") || toon.Equals("JEDIKNIGHTREVAN"))
                    {
                        embeds += createLine("3Z:", gt.zetas[2].ToString());
                    }
                    if (toon.Equals("GLREY") || toon.Equals("SUPREMELEADERKYLOREN") || toon.Equals("GENERALSKYWALKER"))
                    {
                        embeds += createLine("4Z:", gt.zetas[3].ToString());
                    }
                    if (toon.Equals("GLREY") || toon.Equals("SUPREMELEADERKYLOREN"))
                    {
                        embeds += createLine("5Z:", gt.zetas[4].ToString());
                        embeds += createLine("6Z:", gt.zetas[5].ToString());
                    }
                }
            }
            embeds += "```";
            embed.AddField($"{title}", embeds, false);
            #endregion
            if (!gt.isShip)
            {
                #region gear
                title = "==Gear==\n";
                embeds = "```CSS\n";
                embeds += createLine("G13:", $"{gt.gearLevel[8]}");
                embeds += createLine("G12+5:", $"{gt.gearLevel[7]}");
                embeds += createLine("G12+4:", $"{gt.gearLevel[6]}");
                embeds += createLine("G12+3:", $"{gt.gearLevel[5]}");
                embeds += createLine("G12+2:", $"{gt.gearLevel[4]}");
                embeds += createLine("G12+1:", $"{gt.gearLevel[3]}");
                embeds += createLine("G12:", $"{gt.gearLevel[2]}");
                embeds += createLine("G11:", $"{gt.gearLevel[1]}");
                embeds += createLine("G10:", $"{gt.gearLevel[0]}");
                embeds += "```";
                embed.AddField($"{title}", embeds, false);
                #endregion
                #region relics
                title = "==Relics==";
                embeds = "```CSS\n";
                embeds += createLine("Relic 7:", $"{gt.relics[7]}");
                embeds += createLine("Relic 6:", $"{gt.relics[6]}");
                embeds += createLine("Relic 5:", $"{gt.relics[5]}");
                embeds += createLine("Relic 4:", $"{gt.relics[4]}");
                embeds += createLine("Relic 3:", $"{gt.relics[3]}");
                embeds += createLine("Relic 2:", $"{gt.relics[2]}");
                embeds += createLine("Relic 1:", $"{gt.relics[1]}");
                embeds += createLine("Relic 0:", $"{gt.relics[0]}");
                embeds += "```";
                embed.AddField($"{title}", embeds, false);
                #endregion
            }
            Console.WriteLine("Building embeds");
            for (int j = 0; j < 5; j++)
            {
                if (!gt.isShip)
                    if (j == 0)
                        embed.AddField($"==Roster==", "```" + $"★|⚙|± |SPD|Off |Name" +
                                                              $"-----------------------\n" + playerStrings[j] + "```", false);
                    else
                        embed.AddField($"========", "```" + playerStrings[j] + "```", false);
                else
                {
                    if (j == 0)
                        embed.AddField($"==Roster==", "```" + $"★|LVL|  GP  |SPD|Off |Name\n" + playerStrings[j] + "```", false);
                    else
                        embed.AddField($"========", "```" + playerStrings[j] + "```", false);
                }
            }
        }
        public async void exceptionHandler(Exception e, CommandContext ctx, DiscordEmbedBuilder embed)
        {
            if (e.Message.Contains("timed"))
            {
                logCommandInfo("Timeout");
                await ctx.RespondAsync("My parts seem to be all gummed up, please try again later");
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
            }
            else
            {
                if (e.Message.ToLower().Contains("gateway"))
                {
                    logCommandInfo("API issues");
                    await ctx.RespondAsync("Something appears to be wrong with the API, please try again later.");
                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
                }
                else
                {
                    if (e.Message.ToLower().Contains("Unexpected character encountered while parsing value".ToLower()))
                    {
                        logCommandInfo("Parse issues");
                        await ctx.RespondAsync("I had problems parsing the data returned by the API, please try again later.");
                        await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
                    }
                    else
                    {
                        logCommandInfo($"Unknown issues: {e.Message}:{e.StackTrace}:{e.InnerException} ");
                        embed.Title = "Something is wrong!";
                        embed.Description = "I borked, please try again later";
                        await ctx.RespondAsync("", embed: embed);
                        await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
                    }
                }
            }
        }
        public string getIgnoreList(string guildID)
        {
            JObject users;
            JArray user;
            String ignoreList = "";
            try
            {
                //if file exists, parse it
                Console.WriteLine("Reading File");
                users = JObject.Parse(File.ReadAllText(detectOS() + @"Data/Ignore.txt"));
                user = (JArray)users["Guilds"];
                Console.WriteLine("Finding Guild");
                foreach (JObject obj in user.Children())
                {
                    Console.WriteLine("Guild Search");
                    Console.WriteLine(obj.Property("ID").Value.ToString());
                    if (obj.Property("ID").Value.ToString().Equals(guildID))
                    {
                        Console.WriteLine("Guild Found");
                        JArray ignored = (JArray)obj[$"Ignored"];
                        if (ignored != null)
                        {
                            if (ignored.Count > 0)
                            {
                                Console.WriteLine("Ignored List found");
                                foreach (JObject ignoree in ignored.Children())
                                {
                                    Console.WriteLine("AllyCode: " + ignoree["allycode"]);
                                    ignoreList += $"{ignoree["allycode"]},";
                                }
                            }
                        }
                        else
                        {
                            return "";
                        }
                        break;
                    }
                }

                if (ignoreList.Length > 0)
                {
                    return ignoreList;
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
            return ignoreList;
        }

        #region Toon Compare
        public GAPlayer[] getToonCompare(DiscordMessage me, uint[] allycodes, swgohHelpApiHelper helper, string list)
        {
            CharacterDefID d = new CharacterDefID();
            GAPlayer[] toons = new GAPlayer[2];
            GAPlayer player1 = new GAPlayer();
            GAPlayer player2 = new GAPlayer();
            toons[0] = player1;
            toons[1] = player2;
            me.RespondAsync("Retrieving players....");
            Console.WriteLine(allycodes.Length);
            Console.WriteLine(allycodes[0] + ":" + allycodes[1]);
            string statRoster = helper.getStats(getGuildMemberString(allycodes, helper));
            PlayerParse.Player player = JsonConvert.DeserializeObject<PlayerParse.Player>("{\"players\":" + statRoster + "}");
            //File.WriteAllText(@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\players.json", statRoster);
            try
            {
                Console.WriteLine(player.PlayerList.Length);
                me.ModifyAsync(me.Content + $"\n \nBuilding stats for {player.PlayerList[0].Name}");
                getToon(player.PlayerList[0], player1, list);
                me.ModifyAsync(me.Content + $"\n\nBuilding stats for {player.PlayerList[1].Name}");
                getToon(player.PlayerList[1], player2, list);
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
            logCommandInfo($"Building Toon Stats");
            for (int i = 0; i < player.PlayerList.Length; i++)
            {
                toons[i].AllyCode = player.PlayerList[i].AllyCode;
                toons[i].name = player.PlayerList[i].Name;
                toons[i].totGP = (int)player.PlayerList[i].Stats[0].Value;
                toons[i].shipGP = (int)player.PlayerList[i].Stats[2].Value;
                toons[i].toonGP = (int)player.PlayerList[i].Stats[1].Value;
                toons[i].offWon = (int)player.PlayerList[i].Stats[13].Value;
                toons[i].defend = (int)player.PlayerList[i].Stats[14].Value;
                toons[i].under = (int)player.PlayerList[i].Stats[17].Value;
                toons[i].fullClear = (int)player.PlayerList[i].Stats[16].Value;
                toons[i].Banners = (int)player.PlayerList[i].Stats[15].Value;
                int[] speeds = new int[4];

                toons[i].speedMods = speeds;
            }
            return toons;
        }
        public void getToon(PlayerParse.PlayerElement pe, GAPlayer gap, string toonName)
        {
            logCommandInfo($"Fetching Roster Stats for {pe.Name}");
            foreach (PlayerParse.Roster r in pe.Roster)
            {
                // Console.WriteLine("Comparing " + r.DefId);
                ToonCompare tc = new ToonCompare();
                if (toonName.Equals(r.DefId))
                {
                    Console.WriteLine("Found " + toonName);
                    tc.level = (int)r.Level;
                    tc.rarity = (int)r.Rarity;
                    foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { if (skill.Tier == 8) { tc.zetas++; } } }

                    try
                    {
                        if (r.Relic.CurrentTier > 1) { tc.relicTier = (int)(r.Relic.CurrentTier - 2); }
                    }
                    catch { }
                    #region final stats
                    tc.rarity = (int)r.Rarity;
                    Console.WriteLine(r.Stats.Final["Health"]);
                    tc.finalHealth = (int)r.Stats.Final["Health"];
                    Console.WriteLine(tc.finalHealth);
                    tc.finalStrength = (int)r.Stats.Final["Strength"];
                    tc.finalAgility = (int)r.Stats.Final["Agility"];
                    tc.finalTactics = (int)r.Stats.Final["Tactics"];
                    tc.finalSpeed = (int)r.Stats.Final["Speed"];
                    tc.finalPhysDam = (int)r.Stats.Final["Physical Damage"];
                    tc.finalSpecDam = (int)r.Stats.Final["Special Damage"];
                    tc.finalArmor = (double)r.Stats.Final["Armor"];
                    tc.finalResistance = (double)r.Stats.Final["Resistance"];
                    tc.finalArmorPen = (int)r.Stats.Final["Armor Penetration"];
                    tc.finalResPen = (int)r.Stats.Final["Resistance Penetration"];
                    tc.finalDodge = (double)r.Stats.Final["Dodge Chance"];
                    tc.finalDeflect = (double)r.Stats.Final["Deflection Chance"];
                    tc.finalPhysCrit = (double)r.Stats.Final["Physical Critical Chance"];
                    tc.finalSpecCrit = (double)r.Stats.Final["Special Critical Chance"];
                    tc.finalCritDam = (double)r.Stats.Final["Critical Damage"];
                    tc.finalPot = (double)r.Stats.Final["Potency"];
                    tc.finalTen = (double)r.Stats.Final["Tenacity"];
                    tc.finalHealthSteal = (double)r.Stats.Final["Health Steal"];
                    tc.finalProt = (int)r.Stats.Final["Protection"];
                    tc.finalPhysAcc = (double)r.Stats.Final["Physical Accuracy"];
                    tc.finalSpecAcc = (double)r.Stats.Final["Special Accuracy"];
                    tc.finalPhysCritAvoid = (double)r.Stats.Final["Physical Critical Avoidance"];
                    tc.finalSpecCritAvoi = (double)r.Stats.Final["Special Critical Avoidance"];
                    tc.finalMastery = (int)r.Stats.Final["Mastery"];
                    #endregion
                    #region Mod Stats
                    try
                    {
                        tc.modHealth = (int)r.Stats.Mods["Health"];
                    }
                    catch
                    {
                        tc.modHealth = 0;
                    }
                    try
                    {
                        tc.modSpeed = (int)r.Stats.Mods["Speed"];
                    }
                    catch
                    {
                        tc.modSpeed = 0;
                    }
                    try
                    {
                        tc.modPhysicalDamage = (int)r.Stats.Mods["Physical Damage"];
                    }
                    catch
                    {
                        tc.modPhysicalDamage = 0;
                    }
                    try
                    {
                        tc.modSpecialDamage = (int)r.Stats.Mods["Special Damage"];
                    }
                    catch
                    {
                        tc.modSpecialDamage = 0;
                    }
                    try
                    {
                        tc.modArmor = (double)r.Stats.Mods["Armor"];
                    }
                    catch
                    {
                        tc.modArmor = 0;
                    }
                    try
                    {
                        tc.modResistance = (double)r.Stats.Mods["Resistance"];
                    }
                    catch
                    {
                        tc.modResistance = 0;
                    }
                    try
                    {
                        tc.modCritDamage = (double)r.Stats.Mods["Critical Damage"];
                    }
                    catch
                    {
                        tc.modCritDamage = 0;
                    }
                    try
                    {
                        tc.modPotency = (double)r.Stats.Mods["Potency"];
                    }
                    catch
                    {
                        tc.modPotency = 0;
                    }
                    try
                    {
                        tc.modTenacity = (double)r.Stats.Mods["Tenacity"];
                    }
                    catch
                    {
                        tc.modTenacity = 0;
                    }
                    try
                    {
                        tc.modPhysicalCriticalChance = (double)r.Stats.Mods["Physical Critical Chance"];
                    }
                    catch
                    {
                        tc.modPhysicalCriticalChance = 0;
                    }
                    try
                    {
                        tc.modSpecialCriticalChance = (double)r.Stats.Mods["Physical Critical Chance"];
                    }
                    catch
                    {
                        tc.modSpecialCriticalChance = 0;
                    }
                    try
                    {
                        tc.modProtection = (int)r.Stats.Mods["Protection"];
                    }
                    catch
                    {
                        tc.modProtection = 0;
                    }
                    #endregion
                    gap.toonCompare = tc;
                }
            }
        }
        #endregion

        public void calcIgnoredGP(GuildParse.GuildMember guild, TWGuild guildOut)
        {
            foreach (GuildParse.Roster r in guild.Roster)
            {
                if (guildOut.ignoreList.Contains(r.AllyCode.ToString()))
                {
                    guildOut.gpIgnored += (int)r.Gp;
                    Console.WriteLine($"ignoring { guildOut.gpIgnored} Total GP");
                    guildOut.gpIgnoredToon += (double)r.GpChar;
                    Console.WriteLine($"ignoring { guildOut.gpIgnoredToon} Toon GP");
                    guildOut.gpIgnoredFleet += (double)r.GpShip;
                    Console.WriteLine($"ignoring { guildOut.gpIgnoredFleet} Fleet GP");
                }
            }
        }
        public void buildHeader(String header, String sortedBy, GuildParse.GuildMember guild, DiscordEmbedBuilder embed)
        {
            String s = "";
            s = header + sortedBy + "\n";
            s += "======= Overview =======```\n";
            s += createLine("Members:", guild.Members.ToString());
            s += createLine("Total GP:", (guild.Gp / 1000000.0).ToString("0.##") + "M");
            s += createLine("Character GP:", (buildCharGP(guild.Roster, "Char") / 1000000.0).ToString("###.##") + "M");
            s += createLine("Fleet GP:", (buildCharGP(guild.Roster, "Fleet") / 1000000.0).ToString("###.##") + "M");
            s += "```";
            embed.Description = s;
        }
        public string detectOS()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return $@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\";
            }
            return "";
        }
    }
}