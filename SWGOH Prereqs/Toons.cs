using System;
using System.Collections.Generic;
using System.Text;

namespace SWGOH_Prereqs
{
    class Toons
    {
        public int Total { get; set; }
        public int totRel { get; set; }
        public int gp16 { get; set; }
        public int gp20 { get; set; }
        public int z { get; set; }
        public int zz { get; set; }
        public int zzz { get; set; }
        public int zzzz { get; set; }
        public int[] gear = new int[3];
        public int[] stars = new int[3];
        public int[] relics = new int[8];
        public string name { get; set; }
        public string id { get; set; }
        public int maxZetas { get; set; }

        public Toons()
        {
        }
    }
}
