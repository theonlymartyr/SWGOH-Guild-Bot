using System;
using System.Collections.Generic;
using System.Text;
using PlayerParse;

namespace SWGOH
{
    public class SWGOHLegends
    {
        public string CheckRoster(Roster[] r, string toon)
        {
            List<Roster> requiredChars = new List<Roster>();
            int fivestars = 0, sixstars = 0, sevenstars = 0;
            switch (toon.ToLower())
            {
                case "thrawn":
                case "gat":
                case "grand admiral thrawn":
                    foreach (Roster rs in r)
                    {
                        if (rs.DefId.Contains("SABINE") || rs.DefId.Contains("HERA") || rs.DefId.Contains("KANAN") || rs.DefId.Contains("EZRA") || rs.DefId.Contains("CHOPPER") || rs.DefId.Contains("ZEB"))
                        {
                            requiredChars.Add(rs);
                            if (rs.Rarity == 5) { fivestars++; }
                            if (rs.Rarity == 6) { sixstars++; }
                            if (rs.Rarity == 7) { sevenstars++; }
                        }
                    }
                    if (requiredChars.Count == 0) { return "You have not unlocked the required characters."; }
                    if (requiredChars.Count == 0)
                    {
                    }

                    break;
                case "cls":
                case "commander luke":
                case "commander luke skywalker":
                    break;
            }
            return "";
        }
    }
}
