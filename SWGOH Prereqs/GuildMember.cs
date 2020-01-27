using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SWGOH
{
    public class GuildMember
    {
        public int g12 = 0, g13 = 0, relics = 0, totalRelics = 0, relic7 = 0, relic4 = 0, numGP = 0;
        int[] rLevel = new int[8];
        string r7 = "", toonGP = "", shipGP = "", GP = "";
        PlayerParse.PlayerElement p;
        #region toonNames
        Dictionary<string, string> toons = new Dictionary<string, string>()
        {
            { "MAGMATROOPER", "Magma Trooper" },
            {"HERMITYODA", "Hermit Yoda"},
            {"ZEBS3", "Zeb" },
            {"BARRISSOFFEE", "Barriss Offee" },
            {"CT210408", "ECHO" },
            {"B1BATTLEDROIDV2", "B1" },
            {"COLONELSTARCK", "Colonel Starck" },
            {"TIEFIGHTERFOSF", "FOSF Tie Fighter" },
            {"BB8", "BB8" },
            {"POGGLETHELESSER", "Poggle" },
            {"FIRSTORDERSPECIALFORCESPILOT", "FOSF Tie Pilot" },
            {"GENERALKENOBI", "GK" },
            {"JAWASCAVENGER", "Jawa Scavenger" },
            {"L3_37", "L337" },
            {"DROIDEKA", "Droideka" },
            {"EWOKSCOUT", "Ewok Scout" },
            {"SITHINFILTRATOR", "Scimitar" },
            {"K2SO", "K2SO" },
            {"SITHTROOPER", "Sith Trooper" },
            {"IG86SENTINELDROID", "IG 86" },
            {"SITHMARAUDER", "Sith Marauder" },
            {"REYJEDITRAINING", "Rey (Jedi Training)" },
            {"KYLOREN", "Kylo Ren" },
            {"DENGAR", "Dengar" },
            {"ARC170REX", "Arc 170" },
            {"TIEFIGHTERFIRSTORDER", "FO Tie Fighter" },
            {"MAGNAGUARD", "Magnaguard" },
            {"MILLENNIUMFALCONEP7", "Rey's Falcon" },
            {"RANGETROOPER", "Range Trooper" },
            {"QUIGONJINN", "Qui-gon Jinn" },
            {"ROYALGUARD", "Royal Guard" },
            {"GEONOSIANSPY", "Geo Spy" },
            {"BASTILASHAN", "Bastila Shan" },
            {"JAWA", "Jawa" },
            {"SITHFIGHTER", "Sith Fighter" },
            {"XWINGRESISTANCE", "Resistance X-Wing" },
            {"QIRA", "Qi'ra" },
            {"POE", "Poe Dameron" },
            {"TALIA", "Talia" },
            {"NIGHTSISTERZOMBIE", "NS Zombie" },
            {"SCARIFREBEL", "Scarif Rebel Pathfinder" },
            {"GRANDADMIRALTHRAWN", "Thrawn" },
            {"JEDISTARFIGHTERANAKIN", "Eta 2" },
            {"IMPERIALPROBEDROID", "IPD" },
            {"SABINEWRENS3", "Sabine" },
            {"PAPLOO", "Paploo" },
            {"YOUNGHAN", "Young Han" },
            {"EWOKELDER", "Ewok Elder" },
            {"CHIEFCHIRPA", "Chief Chirpa" },
            {"GENERALHUX", "General Hux" },
            {"CT5555", "Fives" },
            {"VISASMARR", "Visas Marr" },
            {"LUKESKYWALKER", "Farmboy Luke" },
            {"KITFISTO", "Kit Fisto" },
            {"HOUNDSTOOTH", "Hound's Tooth" },
            {"ADMINISTRATORLANDO", "Lando" },
            {"ANAKINKNIGHT", "JKA" },
            {"IG2000", "IG-2000" },
            {"NIGHTSISTERSPIRIT", "NS Spirit" },
            {"XANADUBLOOD", "Xanadu Blood" },
            {"CLONESERGEANTPHASEI", "Clone Sergeant" },
            {"ADMIRALACKBAR", "Admiral Ackbar" },
            {"FULCRUMAHSOKA", "Ahsoka (Fulcrum)" },
            {"STORMTROOPER", "Stormtrooper" },
            {"LUMINARAUNDULI", "Luminara" },
            {"JANGOFETT", "Jango Fett" },
            {"HOTHREBELSCOUT", "Hoth Rebel Scout" },
            {"GHOST", "Ghost" },
            {"NUTEGUNRAY", "Nute Gunray" },
            {"SLAVE1", "Slave 1" },
            {"TIEADVANCED", "Tie Advanced" },
            {"VEERS", "General Veers" },
            {"PAO", "Pao" },
            {"IG88", "IG-88" },
            {"MILLENNIUMFALCONPRISTINE", "Lando's Falcon" },
            {"EBONHAWK", "Ebon Hawk" },
            {"REY", "Scavenger Rey" },
            {"BLADEOFDORIN", "Plo Koon's Star Fighter" },
            {"DARTHNIHILUS", "DN" },
            {"EMPERORPALPATINE", "Palpatine" },
            {"PLOKOON", "Plo Koon" },
            {"DAKA", "Daka" },
            {"GRANDMOFFTARKIN", "Tarkin" },
            {"LOGRAY", "Logray" },
            {"GEONOSIANSOLDIER", "Geo Soldier" },
            {"VULTUREDROID", "Vulture Droid" },
            {"RESISTANCETROOPER", "Resistance Trooper" },
            {"HK47", "HK-47" },
            {"TUSKENRAIDER", "Tusken Raider" },
            {"CASSIANANDOR", "Cassian" },
            {"DARTHTRAYA", "Traya" },
            {"BAZEMALBUS", "Baze" },
            {"EPIXFINN", "Res Hero Finn" },
            {"ZAALBAR", "Zaalbar" },
            {"KCLONEWARSCHEWBACCA2SO", "CW Chewbacca" },
            {"COMMANDERLUKESKYWALKER", "CLS" },
            {"B2SUPERBATTLEDROID", "B2" },
            {"UWINGROGUEONE", "Cassian's U-Wing" },
            {"DARTHSIDIOUS", "Sidious" },
            {"OLDBENKENOBI", "Old Ben" },
            {"KYLORENUNMASKED", "Kylo Ren (Unmasked)" },
            {"YOUNGCHEWBACCA", "Young Chewbacca" },
            {"YOUNGLANDO", "Young Lando" },
            {"HANSOLO", "Han Solo" },
            {"SMUGGLERHAN", "Vet Han" },
            {"UGNAUGHT", "Ugnaught" },
            {"FOSITHTROOPER", "FO Sith Trooper" },
            {"UMBARANSTARFIGHTER", "Umbaran" },
            {"WAMPA", "Wampa" },
            {"SMUGGLERCHEWBACCA", "Vet Chewbacca" },
            {"R2D2_LEGENDARY", "R2D2" },
            {"UWINGSCARIF", "Bistan's U-Wing" },
            {"HUMANTHUG", "Mob Enforcer" },
            {"GARSAXON", "Gar Saxon" },
            {"CORUSCANTUNDERWORLDPOLICE", "CUP" },
            {"ENFYSNEST", "Nest" },
            {"MOTHERTALZIN", "MT" },
            {"BIGGSDARKLIGHTER", "Biggs" },
            {"CC2224", "Cody" },
            {"C3POLEGENDARY", "C3PO" },
            {"JEDIKNIGHTREVAN", "JKR" },
            {"PADMEAMIDALA", "Padme" },
            {"NIGHTSISTERINITIATE", "NS Initiate" },
            {"CT7567", "Rex" },
            {"EMPERORSSHUTTLE", "Emperor's Shuttle" },
            {"ZAMWESELL", "Zam" },
            {"CAPITALMONCALAMARICRUISER", "Home One" },
            {"SUNFAC", "Sun Fac" },
            {"LOBOT", "Lobot" },
            {"EMBO", "Embo" },
            {"IMAGUNDI", "Ima Gun Di" },
            {"TIESILENCER", "Tie Silencer" },
            {"YWINGCLONEWARS", "Y-Wing" },
            {"XWINGRED3", "Biggs' X-Wing" },
            {"EETHKOTH", "Eeth Koth" },
            {"CAPITALSTARDESTROYER", "Executrix" },
            {"T3_M4", "T3-M4" },
            {"XWINGRED2", "Wedge's X-Wing" },
            {"JEDISTARFIGHTERAHSOKATANO", "Ahsoka's Star Fighter" },
            {"HOTHREBELSOLDIER", "Hoth Rebel Soldier" },
            {"ASAJVENTRESS", "Asajj Ventress" },
            {"GRIEVOUS", "GG" },
            {"CHIRRUTIMWE", "Chirrut" },
            {"JUHANI", "Juhani" },
            {"CAPITALNEGOTIATOR", "Negotiator" },
            {"KANANJARRUSS3", "Kanan" },
            {"ROSETICO", "Rose Tico" },
            {"JEDIKNIGHTCONSULAR", "Consular" },
            {"AHSOKATANO", "Ahsoka Tano" },
            {"DIRECTORKRENNIC", "Krennic" },
            {"TIEFIGHTERIMPERIAL", "Imperial Tie Fighter" },
            {"STORMTROOPERHAN", "ST Han" },
            {"HERASYNDULLAS3", "Hera" },
            {"BASTILASHANDARK", "BSF" },
            {"BOSSK", "Bossk" },
            {"FIRSTORDEREXECUTIONER", "FOX" },
            {"VADER", "Vader" },
            {"COUNTDOOKU", "Count Dooku" },
            {"MISSIONVAO", "Mission Vao" },
            {"ARC170CLONESERGEANT", "Sergeant's ARC 170" },
            {"GEONOSIANBROODALPHA", "GBA" },
            {"MAUL", "Darth Maul" },
            {"JOLEEBINDO", "Jolee" },
            {"CHIEFNEBIT", "Chief Nebit" },
            {"TUSKENSHAMAN", "Tusken Shaman" },
            {"IMPERIALSUPERCOMMANDO", "Imperial Super Commando" },
            {"FIRSTORDERTIEPILOT", "FO Tie Pilot" },
            {"DARTHSION", "Sion" },
            {"CAPITALJEDICRUISER", "Endurance" },
            {"CAPITALCHIMAERA", "Chimaera" },
            {"COMMANDSHUTTLE", "Command Shuttle" },
            {"FIRSTORDERTROOPER", "FO Stormtrooper" },
            {"PHANTOM2", "Phantom" },
            {"HOTHLEIA", "ROLO" },
            {"GREEDO", "Greedo" },
            {"BODHIROOK", "Bodhi Rook" },
            {"CHEWBACCALEGENDARY", "Chewbacca" },
            {"CHOPPERS3", "Chopper" },
            {"BOBAFETT", "Boba Fett" },
            {"WEDGEANTILLES", "Wedge" },
            {"DEATHTROOPER", "Death Trooper" },
            {"CARTHONASI", "Carth" },
            {"ARCTROOPER501ST", "ARC Trooper" },
            {"WICKET", "Wicket" },
            {"GEONOSIANSTARFIGHTER2", "Geo Soldier's Starfighter" },
            {"GEONOSIANSTARFIGHTER1", "Sun Fac's Starfighter" },
            {"GEONOSIANSTARFIGHTER3", "Geo Spy's Starfighter" },
            {"MACEWINDU", "Mace" },
            {"DATHCHA", "Datcha" },
            {"JYNERSO", "Jyn Erso" },
            {"TEEBO", "Teebo" },
            {"XWINGBLACKONE", "Poe's X-Wing" },
            {"SITHBOMBER", "B28" },
            {"AMILYNHOLDO", "Holdo" },
            {"GAUNTLETSTARFIGHTER", "Gauntlet Starfighter" },
            {"SITHASSASSIN", "Sith Assassin" },
            {"SNOWTROOPER", "Snowtrooper" },
            {"SHORETROOPER", "Shoretrooper" },
            {"PRINCESSLEIA", "Princess Leia" },
            {"SAVAGEOPRESS", "Savage" },
            {"FINN", "Finn" },
            {"PHASMA", "Captain Phasma" },
            {"EZRABRIDGERS3", "Ezra" },
            {"TIEREAPER", "Tie Reaper" },
            {"HYENABOMBER", "Hyena Bomber" },
            {"JEDISTARFIGHTERCONSULAR", "Consular's Starfighter" },
            {"FIRSTORDEROFFICERMALE", "FO Officer" },
            {"RESISTANCEPILOT", "Resistance Pilot" },
            {"AURRA_SING", "Aurra Sing" },
            {"JEDIKNIGHTGUARDIAN", "JKG" },
            {"CANDEROUSORDO", "Canderous" },
            {"CADBANE", "Cad Bane" },
            {"TIEFIGHTERPILOT", "TFP" },
            {"GRANDMASTERYODA", "GMY" },
            {"SHAAKTI", "Shaak Ti" },
            {"BISTAN", "Bistan" },
            {"DARTHMALAK", "Malak" },
            {"JAWAENGINEER", "Jawa Engineer" },
            {"AAYLASECURA", "Aayla Secura" },
            {"WATTAMBOR", "Wat Tambor" },
            {"DARTHREVAN", "DR" },
            {"URORRURRR", "URoRRuR'R'R" },
            {"NIGHTSISTERACOLYTE", "NS Acolyte" },
            {"GENERALSKYWALKER", "Gen Skywalker" },
            {"HOTHHAN", "Captain Han" },
            {"GAMORREANGUARD", "Gamorrean Guard" }
        };
        #endregion
        public GuildMember() { }
        public GuildMember(PlayerParse.PlayerElement pe) { p = pe; }
        public void calcstats()
        {
            foreach (PlayerParse.Roster rost in p.Roster)
            {
                if (rost.Gear == 12) { g12++; }
                if (rost.Gear == 13)
                {
                    g13++;
                    if (rost.Relic.CurrentTier > 1)
                    {
                        relics++; rLevel[rost.Relic.CurrentTier - 2]++;
                        if ((rost.Relic.CurrentTier - 2) == 7) { r7 += "." + toons.GetValueOrDefault(rost.DefId) + "\n"; }
                    }
                }
            }
            GP = String.Format(CultureInfo.InvariantCulture, "{0:#,##,M}", (double)p.Stats[0].Value);
            toonGP = p.Stats[1].Value.ToString("#,##,M", CultureInfo.InvariantCulture);
            shipGP = p.Stats[2].Value.ToString("#,##,M", CultureInfo.InvariantCulture);
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
