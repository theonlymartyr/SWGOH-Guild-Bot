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
    class ToonCompare
    {
        public int level { get; set; }
        public int rarity { get; set; }
        public int zetas { get; set; }
        public int relicTier { get; set; }
        public bool ship = false;


        public int modHealth { get; set; }
        public int modSpeed { get; set; }
        public int modPhysicalDamage { get; set; }
        public int modSpecialDamage { get; set; }
        public double modArmor { get; set; }
        public double modResistance { get; set; }
        public double modCritDamage { get; set; }
        public double modPotency { get; set; }
        public double modTenacity { get; set; }
        public double modPhysicalCriticalChance { get; set; }
        public double modSpecialCriticalChance { get; set; }
        public int modProtection { get; set; }
        public double modSpecCritAvoid { get; set; }
        public double modPhysCritAvoid { get; set; }

        public int finalHealth { get; set; }
        public int finalStrength { get; set; }
        public int finalAgility { get; set; }
        public int finalTactics { get; set; }
        public int finalSpeed { get; set; }
        public int finalPhysDam { get; set; }
        public int finalSpecDam { get; set; }
        public double finalArmor { get; set; }
        public double finalResistance { get; set; }
        public int finalArmorPen { get; set; }
        public int finalResPen { get; set; }
        public double finalDodge { get; set; }
        public double finalDeflect { get; set; }
        public double finalPhysCrit { get; set; }
        public double finalSpecCrit { get; set; }
        public double finalCritDam { get; set; }
        public double finalPot { get; set; }
        public double finalTen { get; set; }
        public double finalHealthSteal { get; set; }
        public int finalProt { get; set; }
        public double finalPhysAcc { get; set; }
        public double finalSpecAcc { get; set; }
        public double finalPhysCritAvoid { get; set; }
        public double finalSpecCritAvoi { get; set; }
        public int finalMastery { get; set; }
    }
}
