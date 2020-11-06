using System;
using System.Collections.Generic;
using System.Text;

namespace SWGOH_Prereqs
{
    class GAPlayer
    {
        public List<GAToon> toonsOwned = new List<GAToon>();
        public ToonCompare toonCompare;
        public long AllyCode { get; set; }
        public int Zetas { get; set; }
        public int totalRelics { get; set; }
        public int off100 { get; set; }
        public int[] relics = new int[8];
        public int G11 { get; set; }
        public int G13 { get; set; }
        public int G12 { get; set; }
        public int G121 { get; set; }
        public int G122 { get; set; }
        public int G123 { get; set; }
        public int G124 { get; set; }
        public int G125 { get; set; }
        public int[] speedMods = new int[4];
        public int sixStarMods = 0;
        public string name { get; set; }
        public int totGP { get; set; }
        public int toonGP { get; set; }
        public int shipGP { get; set; }
        public int offWon { get; set; }
        public int defend { get; set; }
        public int under { get; set; }
        public int fullClear { get; set; }
        public int Banners { get; set; }

        public GAPlayer() { }
    }
}
