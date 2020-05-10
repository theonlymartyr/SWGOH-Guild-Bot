using System;
using System.Collections.Generic;
using System.Text;

namespace SWGOH_Prereqs
{
    public class ToonStats
    {
        #region variables
        public int G11 = 0, G12 = 0, G121 = 0, G122 = 0, G123 = 0, G124 = 0, G125 = 0, G13 = 0, TotalRelics = 0, zetas = 0, off100 = 0;
        public int[] relics = new int[8];
        public int[] speedMods = new int[4];
        public int sixStarMods = 0;
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
        public GAS gas;
        public Rey rey;
        public SLKR slkr;
        #endregion
        public ToonStats() { ht = new HT(); mf = new MF(); nego = new Nego(); traya = new Traya(); en = new EN(); padme = new Padme(); jkr = new JKR(); grievous = new Grievous(); bsf = new BSF(); bossk = new Bossk(); gba = new GBA(); mal = new Malevolence(); dr = new DR(); dm = new DM(); gas = new GAS(); rey = new Rey(); slkr = new SLKR(); }
        #region ToonObjects
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
            public int z, zz;
        }
        public partial class GAS
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
            public int z, zz, zzz, zzzz;
        }
        public partial class Rey
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
            public int z, zz, zzz, zzzz, zzzzz, zzzzzz;
        }
        public partial class SLKR
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
            public int z, zz, zzz, zzzz, zzzzz, zzzzzz;
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
            public int z, zz;
        }
        public partial class JKR
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
            public int z, zz, zzz;
        }
        public partial class Grievous
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
            public int z, zz;
        }
        public partial class BSF
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
            public int z;
        }
        public partial class Bossk
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
            public int z, zz;
        }
        public partial class GBA
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
            public int z, zz;
        }
        public partial class DR
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
            public int z, zz, zzz;
        }
        public partial class DM
        {
            public int Total, totRel, gp16, gp20;
            public int[] gear = new int[3];
            public int[] relics = new int[8];
            public int[] stars = new int[3];
            public int z, zz;
        }
        #endregion
    }
}
