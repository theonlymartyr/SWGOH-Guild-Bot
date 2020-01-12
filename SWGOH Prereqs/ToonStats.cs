using System;
using System.Collections.Generic;
using System.Text;

namespace SWGOH_Prereqs
{
    public class ToonStats
    {
        public int G11 = 0, G12 = 0, G121 = 0, G122 = 0, G123 = 0, G124 = 0, G125 = 0, G13 = 0, TotalRelics = 0, zetas = 0;
        public int[] relics = new int[8];
        public HT ht;
        public MF mf;
        public Nego nego;
        public Traya traya;
        public EN en;
        public Padme padme;
        public JKR jkr;
        public Grievous grievous;
        public BSF bsf;
        public Bossk bossk;
        public GBA gba;
        public Malevolence mal;
        public DR dr;
        public DM dm;
        public ToonStats() { ht = new HT(); mf = new MF(); nego = new Nego(); traya = new Traya(); en = new EN(); padme = new Padme(); jkr = new JKR(); grievous = new Grievous(); bsf = new BSF(); bossk = new Bossk(); gba = new GBA(); mal = new Malevolence(); dr = new DR(); dm = new DM(); }
        public partial class HT
        {
            public int Total;
            public int[] stars = new int[3];

        }
        public partial class MF
        {
            public int Total;
            public int[] stars = new int[3];
        }
        public partial class Nego
        {
            public int Total;
            public int[] stars = new int[3];
        }
        public partial class Malevolence
        {
            public int Total;
            public int[] stars = new int[3];
        }
        public partial class Traya
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
        }
        public partial class EN
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
        }
        public partial class Padme
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
        }
        public partial class JKR
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
        }
        public partial class Grievous
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
        }
        public partial class BSF
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
        }
        public partial class Bossk
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
        }
        public partial class GBA
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
        }
        public partial class DR
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
        }
        public partial class DM
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
        }
    }
}
