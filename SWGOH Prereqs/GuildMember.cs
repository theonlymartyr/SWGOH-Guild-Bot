using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SWGOH_Prereqs;

namespace SWGOH
{
    public class GuildMember
    {
        public int g12 = 0, g13 = 0, relics = 0, totalRelics = 0, relic7 = 0, relic4 = 0, numGP = 0;
        int[] rLevel = new int[15];
        string r7 = "", toonGP = "", shipGP = "", GP = "";
        PlayerParse.PlayerElement p;
        public GuildMember() { }
        public GuildMember(PlayerParse.PlayerElement pe) { p = pe; }
        public void calcstats()
        {
            CharacterDefID d = new CharacterDefID();

            foreach (PlayerParse.Roster rost in p.Roster)
            {
                if (rost.Gear == 12) { g12++; }
                if (rost.Gear == 13)
                {
                    g13++;
                    if (rost.Relic.CurrentTier > 1)
                    {
                        relics++; rLevel[rost.Relic.CurrentTier - 2]++;
                        // if ((rost.Relic.CurrentTier - 2) == 7) { r7 += "." + d.toons.GetValueOrDefault(rost.DefId) + "\n"; }
                    }
                }
            }
            GP = String.Format(CultureInfo.InvariantCulture, "{0:#,##,M}", (double)p.Stats[0].Value);
            if (p.Stats[1].Value >= 1000000) { toonGP = p.Stats[1].Value.ToString("#,###,M", CultureInfo.InvariantCulture); }
            else { toonGP = p.Stats[1].Value.ToString("#,###,K", CultureInfo.InvariantCulture); }
            if (p.Stats[2].Value >= 1000000) { shipGP = p.Stats[2].Value.ToString("#,###,M", CultureInfo.InvariantCulture); }
            else { shipGP = p.Stats[2].Value.ToString("#,###,k", CultureInfo.InvariantCulture); }
            relic7 = rLevel[7];
            relic4 = rLevel[7] + rLevel[6] + rLevel[5] + rLevel[4];
        }
        public string buildEmbedField()
        {
            string s = "```CSS\n";
            s += createLine("GP:", GP);
            s += createLine("Toon GP:", toonGP);
            s += createLine("Ship GP:", shipGP);
            s += createLine("Tot Relics:", (relics).ToString());
            s += createLine("R7:", relic7.ToString());
            if (r7.Length > 0) { s += createLine("", r7.TrimEnd('\n')); }
            s += createLine("R4+:", relic4.ToString());
            s += createLine("G13:", g13.ToString());
            s += createLine("G12:", g12.ToString());
            s += "```";
            return s;
        }
        public double getGP() { return p.Stats[0].Value; }
        public string getMemberName() { return p.Name; }
        public string createLine(string category, string left)
        {
            if (category.Length > 0) { return category + addDots(10 - category.Length) + addDots(6 - left.Length) + left + addPadding(2) + "\n"; }
            else { return category + left + addPadding(2) + "\n"; }
        }
        public string createHeaderLine(string category, string left, string right)
        {
            return category + addDots(13 - category.Length) + addDots(10 - left.Length) + left + addPadding(2) + "::" + addPadding(2) + right + "\n";
        }
        public string createLine(string category, string left, string right)
        {
            return category + addDots(12 - category.Length - left.Length) + left + "::" + right + "\n";
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
    }
}
