using System;
using System.Collections.Generic;
using System.Text;

namespace SWGOH_Prereqs
{
    class GAToon
    {
        public int GP { get; set; }
        public int starLevel { get; set; }
        public int speed { get; set; }
        public int physDam { get; set; }
        public int specDam { get; set; }
        public string name { get; set; }
        public string igName { get; set; }
        public int gearLevel { get; set; }
        public int gearEquipped { get; set; }
        public int Zetas { get; set; }
        public int relic { get; set; }
        public bool isShip { get; set; }

        public GAToon(String iGName)
        {
            GP = 0;
            starLevel = 0;
            speed = 0;
            physDam = 0;
            specDam = 0;
            name = "";
            igName = iGName;
            gearLevel = 0;
            gearEquipped = 0;
            Zetas = 0;
            relic = 0;
            isShip = false;
        }
    }
}
