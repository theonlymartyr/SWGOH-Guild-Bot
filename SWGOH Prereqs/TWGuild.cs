using System;
using System.Collections.Generic;
using System.Text;

namespace SWGOH_Prereqs
{
    class TWGuild
    {
        public List<Toons> toonList;
        public int G11 = 0, G12 = 0, G121 = 0, G122 = 0, G123 = 0, G124 = 0, G125 = 0, G13 = 0, TotalRelics = 0, zetas = 0, off100 = 0;
        public int[] relics = new int[8];
        public int[] speedMods = new int[4];
        public int sixStarMods = 0;
        public List<string> ignoreList;
        public int gpIgnored { get; set; }
        public double gpIgnoredToon { get; set; }
        public double gpIgnoredFleet { get; set; }
    }
}
