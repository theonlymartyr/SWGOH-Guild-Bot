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
        public int zzzzz { get; set; }
        public int zzzzzz { get; set; }
        public int[] gear = new int[3];
        public int[] stars = new int[3];
        public int[] relics = new int[8];
        public string name { get; set; }
        public string id { get; set; }
        public int maxZetas { get; set; }
        public int[] numZetas;
        public bool ship = false;

        public Toons(String igname, int maxZetas)
        {
            id = igname;
            Total = 0;
            totRel = 0;
            gp16 = 0;
            gp20 = 0;
            numZetas = new int[maxZetas];
        }
    }
}
