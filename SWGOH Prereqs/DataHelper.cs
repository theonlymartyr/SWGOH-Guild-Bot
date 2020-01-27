using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using SWGOH;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;

namespace SWGOH_Prereqs
{
    class DataHelper
    {
        public DataHelper()
        {

        }
        public bool checkCommand(string command, IReadOnlyDictionary<string, Command> commands)
        {
            foreach (string s in commands.Keys)
            {
                if (command.ToLower().Trim().Equals(s)) { return true; }
            }
            return false;
        }
        public void SortMembers(List<GuildMember> gm, int sort, string sortType)
        {
            if (sortType.Equals("RELICS")) { gm.Sort((x, y) => x.relics.CompareTo(y.relics)); }
            if (sortType.Equals("GP")) { gm.Sort((x, y) => x.getGP().CompareTo(y.getGP())); }
            if (sortType.Equals("NAME")) { gm.Sort((x, y) => x.getMemberName().CompareTo(y.getMemberName())); }
            if (sort == 1) { gm.Reverse(); }
            Console.WriteLine("Sorting by: " + sortType);
        }
        public string buildPlayerStats(PlayerParse.Player r, DiscordEmbedBuilder b, DiscordEmbedBuilder b2, string sort, int guildID)
        {
            Database db = new Database();
            string sortedBy = "";
            int i = 0;
            List<GuildMember> gm = new List<GuildMember>();
            foreach (PlayerParse.PlayerElement pe in r.PlayerList)
            {
                GuildMember g = new GuildMember(pe);
                g.calcstats();
                gm.Add(g);
                Hashtable hash = new Hashtable();
                hash.Add("GuildID", guildID);
                hash.Add("Allycode", pe.AllyCode);
                db.InsertRow(hash, "GuildMembers");
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

        public void getGuildStats(PlayerParse.Player guild, ToonStats ts)
        {
            Database db = new Database();
            try
            {
                int[] speeds = new int[4];
                foreach (PlayerParse.PlayerElement pe in guild.PlayerList)
                {
                    foreach (PlayerParse.Roster r in pe.Roster)
                    {
                        int zetas = 0;
                        switch (r.DefId)
                        {
                            #region Toon switch (rework later?)
                            case "HOUNDSTOOTH":
                                if (r.Rarity >= 5) { ts.ht.stars[r.Rarity - 5]++; }
                                ts.ht.Total++;
                                break;
                            case "DARTHTRAYA":
                                if (r.Rarity >= 5) { ts.traya.stars[r.Rarity - 5]++; }
                                if (r.Gear >= 11) { ts.traya.gear[r.Gear - 11]++; }
                                ts.traya.Total++;
                                if (r.Relic.CurrentTier > 1) { ts.traya.relics[r.Relic.CurrentTier - 2]++; ts.traya.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.traya.gp20++;
                                    else
                                        ts.traya.gp16++;
                                }
                                foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { zetas++; } }
                                if (zetas == 1) ts.traya.z++;
                                if (zetas == 2) ts.traya.zz++;
                                break;

                            case "BASTILASHANDARK":
                                if (r.Rarity >= 5) { ts.bsf.stars[r.Rarity - 5]++; }
                                ts.bsf.Total++;
                                if (r.Gear >= 11) { ts.bsf.gear[r.Gear - 11]++; }
                                if (r.Relic.CurrentTier > 1) { ts.bsf.relics[r.Relic.CurrentTier - 2]++; ts.bsf.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.bsf.gp20++;
                                    else
                                        ts.bsf.gp16++;
                                }
                                foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { zetas++; } }
                                if (zetas == 1) ts.bsf.z++;
                                break;
                            case "ENFYSNEST":
                                if (r.Rarity >= 5) { ts.en.stars[r.Rarity - 5]++; }
                                ts.en.Total++;
                                if (r.Gear >= 11) { ts.en.gear[r.Gear - 11]++; }
                                if (r.Relic.CurrentTier > 1) { ts.en.relics[r.Relic.CurrentTier - 2]++; ts.en.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.en.gp20++;
                                    else
                                        ts.en.gp16++;
                                }
                                break;
                            case "PADMEAMIDALA":
                                if (r.Rarity >= 5) { ts.padme.stars[r.Rarity - 5]++; }
                                ts.padme.Total++;
                                if (r.Gear >= 11) { ts.en.gear[r.Gear - 11]++; }
                                if (r.Relic.CurrentTier > 1) { ts.padme.relics[r.Relic.CurrentTier - 2]++; ts.padme.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.padme.gp20++;
                                    else
                                        ts.padme.gp16++;
                                }
                                foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { zetas++; } }
                                if (zetas == 1) ts.padme.z++;
                                if (zetas == 2) ts.padme.zz++;
                                break;
                            case "JEDIKNIGHTREVAN":
                                if (r.Rarity >= 5) { ts.jkr.stars[r.Rarity - 5]++; }
                                ts.jkr.Total++;
                                if (r.Gear >= 11) { ts.jkr.gear[r.Gear - 11]++; }
                                if (r.Relic.CurrentTier > 1) { ts.jkr.relics[r.Relic.CurrentTier - 2]++; ts.jkr.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.jkr.gp20++;
                                    else
                                        ts.jkr.gp16++;
                                }
                                foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { zetas++; } }
                                if (zetas == 1) ts.jkr.z++;
                                if (zetas == 2) ts.jkr.zz++;
                                if (zetas == 3) ts.jkr.zzz++;
                                break;
                            case "GRIEVOUS":
                                if (r.Rarity >= 5) { ts.grievous.stars[r.Rarity - 5]++; }
                                ts.grievous.Total++;
                                if (r.Gear >= 11) { ts.grievous.gear[r.Gear - 11]++; }
                                if (r.Relic.CurrentTier > 1) { ts.grievous.relics[r.Relic.CurrentTier - 2]++; ts.grievous.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.grievous.gp20++;
                                    else
                                        ts.grievous.gp16++;
                                }
                                foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { zetas++; } }
                                if (zetas == 1) ts.grievous.z++;
                                if (zetas == 2) ts.grievous.zz++;
                                break;
                            case "BOSSK":
                                if (r.Rarity >= 5) { ts.bossk.stars[r.Rarity - 5]++; }
                                ts.bossk.Total++;
                                if (r.Gear >= 11) { ts.bossk.gear[r.Gear - 11]++; }
                                if (r.Relic.CurrentTier > 1) { ts.bossk.relics[r.Relic.CurrentTier - 2]++; ts.bossk.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.bossk.gp20++;
                                    else
                                        ts.bossk.gp16++;
                                }
                                foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { zetas++; } }
                                if (zetas == 1) ts.bossk.z++;
                                if (zetas == 2) ts.bossk.zz++;
                                break;
                            case "GEONOSIANBROODALPHA":
                                if (r.Rarity >= 5) { ts.gba.stars[r.Rarity - 5]++; }
                                ts.gba.Total++;
                                if (r.Gear >= 11) { ts.gba.gear[r.Gear - 11]++; }
                                if (r.Relic.CurrentTier > 1) { ts.gba.relics[r.Relic.CurrentTier - 2]++; ts.gba.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.gba.gp20++;
                                    else
                                        ts.gba.gp16++;
                                }
                                foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { zetas++; } }
                                if (zetas == 1) ts.gba.z++;
                                if (zetas == 2) ts.gba.zz++;
                                break;
                            case "DARTHREVAN":
                                if (r.Rarity >= 5) { ts.dr.stars[r.Rarity - 5]++; }
                                ts.dr.Total++;
                                if (r.Gear >= 11) { ts.dr.gear[r.Gear - 11]++; }
                                if (r.Relic.CurrentTier > 1) { ts.dr.relics[r.Relic.CurrentTier - 2]++; ts.dr.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.dr.gp20++;
                                    else
                                        ts.dr.gp16++;
                                }
                                foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { zetas++; } }
                                if (zetas == 1) ts.dr.z++;
                                if (zetas == 2) ts.dr.zz++;
                                if (zetas == 3) ts.dr.zzz++;
                                break;
                            case "DARTHMALAK":
                                if (r.Rarity >= 5) { ts.dm.stars[r.Rarity - 5]++; }
                                ts.dm.Total++;
                                if (r.Gear >= 11) { ts.dm.gear[r.Gear - 11]++; }
                                if (r.Relic.CurrentTier > 1) { ts.dm.relics[r.Relic.CurrentTier - 2]++; ts.dm.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.dm.gp20++;
                                    else
                                        ts.dm.gp16++;
                                }
                                foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { zetas++; } }
                                if (zetas == 1) ts.dm.z++;
                                if (zetas == 2) ts.dm.zz++;
                                break;
                            case "CAPITALNEGOTIATOR":
                                if (r.Rarity >= 5) { ts.nego.stars[r.Rarity - 5]++; }
                                ts.nego.Total++;
                                break;
                            case "CAPITALMALEVOLENCE":
                                if (r.Rarity >= 5) { ts.mal.stars[r.Rarity - 5]++; }
                                ts.mal.Total++;
                                break;
                            case "MILLENNIUMFALCON":
                                if (r.Rarity >= 5) { ts.mf.stars[r.Rarity - 5]++; }
                                ts.mf.Total++;
                                break;
                            case "GENERALSKYWALKER":
                                if (r.Rarity >= 5) { ts.gas.stars[r.Rarity - 5]++; }
                                ts.gas.Total++;
                                if (r.Gear >= 11) { ts.gas.gear[r.Gear - 11]++; }
                                if (r.Relic.CurrentTier > 1) { ts.gas.relics[r.Relic.CurrentTier - 2]++; ts.gas.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.gas.gp20++;
                                    else
                                        ts.gas.gp16++;
                                }
                                foreach (PlayerParse.Skill skill in r.Skills) { if (skill.IsZeta) { zetas++; } }
                                if (zetas == 1) ts.gas.z++;
                                if (zetas == 2) ts.gas.zz++;
                                if (zetas == 3) ts.gas.zzz++;
                                if (zetas == 4) ts.gas.zzzz++;
                                break;
                                #endregion
                        }
                        foreach (PlayerParse.Skill skill in r.Skills)
                        {
                            if (skill.IsZeta) { ts.zetas++; }
                        }
                        foreach (PlayerParse.Mod m in r.Mods)
                        {
                            foreach (PlayerParse.SecondaryStat s in m.SecondaryStat)
                            {
                                if (s.UnitStat == 5) { checkModSpeed(s.Value, speeds); }
                                if (s.UnitStat == 41) { if (s.Value >= 100) { ts.off100++; } }
                            }
                            if (m.Tier == 6) { ts.sixStarMods++; }
                        }
                        switch (r.Gear)
                        {
                            case 11:
                                ts.G11++;
                                break;
                            case 12:
                                switch (r.Equipped.Length)
                                {
                                    case 0:
                                        ts.G12++;
                                        break;
                                    case 1:
                                        ts.G121++;
                                        break;
                                    case 2:
                                        ts.G122++;
                                        break;
                                    case 3:
                                        ts.G123++;
                                        break;
                                    case 4:
                                        ts.G124++;
                                        break;
                                    case 5:
                                        ts.G125++;
                                        break;
                                }
                                break;
                            case 13:
                                ts.G13++;
                                break;
                        }
                        if (r.Relic != null) { if (r.Relic.CurrentTier - 2 >= 0) { ts.TotalRelics++; ts.relics[r.Relic.CurrentTier - 2]++; } }
                    }
                }
                ts.speedMods = speeds;
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
        }
        public uint[][] buildMemberArray(GuildParse.Roster[] r)
        {
            uint[] members1 = new uint[25], members2 = new uint[r.Length - 25];
            uint[][] totalMembers = new uint[2][];
            for (int i = 0; i < r.Length; i++)
            {
                if (i < 25) { members1[i] = r[i].AllyCode; }
                else { members2[i - 25] = r[i].AllyCode; }
            }
            totalMembers[0] = members1;
            totalMembers[1] = members2;
            Console.WriteLine("# allycodes sent: " + members1.Length + " - " + members2.Length);
            return totalMembers;
        }
        public long buildCharGP(GuildParse.Roster[] r, string toonType)
        {
            long GP = 0;
            if (toonType.Equals("Char")) { foreach (GuildParse.Roster rost in r) { GP += rost.GpChar; } }
            if (toonType.Equals("Fleet")) { foreach (GuildParse.Roster rost in r) { GP += rost.GpShip; } }
            return GP / 1000000;
        }
        #region helper functions
        public string createLineDocs(string category, string left)
        {
            if (category.Length > 0) { return "" + category + addPadding(12 - category.Length) + " :: " + addPadding(4 - left.Length) + left + addPadding(2) + "\n"; }
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
        public string createLongLine(string category, string left, string right)
        {
            return category + addDots(20 - category.Length - left.Length) + left + addPadding(2) + "::" + addPadding(2) + right + "\n";
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
        public PlayerParse.Player getGuildMembers(uint[][] ac, swgohHelpApiHelper helper)
        {
            dynamic obj = new ExpandoObject();
            obj.name = 1;
            obj.roster = 1;
            obj.stats = 1;

            string guild = helper.fetchPlayer(ac[0], null, null, obj);
            guild = guild.Remove(guild.LastIndexOf("]")) + "," + helper.fetchPlayer(ac[1], null, null, obj).Substring(1);

            guild = "{\"players\":" + guild + "}";
            File.WriteAllText(@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\players.txt", guild);
            PlayerParse.Player player = JsonConvert.DeserializeObject<PlayerParse.Player>(guild);
            return player;
        }
        public GuildParse.Guild getGuild(uint[] ac, swgohHelpApiHelper helper)
        {
            string guild = helper.fetchGuild(ac);
            guild = "{\"guild\":" + guild + "}";
            GuildParse.Guild gi = JsonConvert.DeserializeObject<GuildParse.Guild>(guild);
            return gi;
        }
        public void checkModSpeed(double value, int[] speeds)
        {
            if (value >= 10)
            {
                if (value >= 15)
                {
                    if (value >= 20)
                    {
                        if (value >= 25)
                        {
                            speeds[3]++;
                        }
                        speeds[2]++;
                    }
                    speeds[1]++;
                }
                speeds[0]++;
            }
        }
    }
}
