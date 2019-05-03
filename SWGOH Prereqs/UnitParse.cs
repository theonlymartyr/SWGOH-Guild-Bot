using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Units
{
    public partial class Units
    {
        [JsonProperty("results")]
        public Result[] Results { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("MAGMATROOPER")]
        public Unit[] Magmatrooper { get; set; }

        [JsonProperty("HERMITYODA")]
        public Unit[] Hermityoda { get; set; }

        [JsonProperty("ZEBS3")]
        public Unit[] Zebs3 { get; set; }

        [JsonProperty("BARRISSOFFEE")]
        public Unit[] Barrissoffee { get; set; }

        [JsonProperty("CT210408")]
        public Unit[] Ct210408 { get; set; }

        [JsonProperty("B1BATTLEDROIDV2")]
        public Unit[] B1Battledroidv2 { get; set; }

        [JsonProperty("COLONELSTARCK")]
        public Unit[] Colonelstarck { get; set; }

        [JsonProperty("TIEFIGHTERFOSF")]
        public Unit[] Tiefighterfosf { get; set; }

        [JsonProperty("BB8")]
        public Unit[] Bb8 { get; set; }

        [JsonProperty("POGGLETHELESSER")]
        public Unit[] Pogglethelesser { get; set; }

        [JsonProperty("FIRSTORDERSPECIALFORCESPILOT")]
        public Unit[] Firstorderspecialforcespilot { get; set; }

        [JsonProperty("GENERALKENOBI")]
        public Unit[] Generalkenobi { get; set; }

        [JsonProperty("JAWASCAVENGER")]
        public Unit[] Jawascavenger { get; set; }

        [JsonProperty("L3_37")]
        public Unit[] L337 { get; set; }

        [JsonProperty("DROIDEKA")]
        public Unit[] Droideka { get; set; }

        [JsonProperty("EWOKSCOUT")]
        public Unit[] Ewokscout { get; set; }

        [JsonProperty("SITHINFILTRATOR")]
        public Unit[] Sithinfiltrator { get; set; }

        [JsonProperty("K2SO")]
        public Unit[] K2So { get; set; }

        [JsonProperty("IG86SENTINELDROID")]
        public Unit[] Ig86Sentineldroid { get; set; }

        [JsonProperty("SITHMARAUDER")]
        public Unit[] Sithmarauder { get; set; }

        [JsonProperty("REYJEDITRAINING")]
        public Unit[] Reyjeditraining { get; set; }

        [JsonProperty("KYLOREN")]
        public Unit[] Kyloren { get; set; }

        [JsonProperty("DENGAR")]
        public Unit[] Dengar { get; set; }

        [JsonProperty("ARC170REX")]
        public Unit[] Arc170Rex { get; set; }

        [JsonProperty("TIEFIGHTERFIRSTORDER")]
        public Unit[] Tiefighterfirstorder { get; set; }

        [JsonProperty("MAGNAGUARD")]
        public Unit[] Magnaguard { get; set; }

        [JsonProperty("RANGETROOPER")]
        public Unit[] Rangetrooper { get; set; }

        [JsonProperty("QUIGONJINN")]
        public Unit[] Quigonjinn { get; set; }

        [JsonProperty("ROYALGUARD")]
        public Unit[] Royalguard { get; set; }

        [JsonProperty("GEONOSIANSPY")]
        public Unit[] Geonosianspy { get; set; }

        [JsonProperty("BASTILASHAN")]
        public Unit[] Bastilashan { get; set; }

        [JsonProperty("JAWA")]
        public Unit[] Jawa { get; set; }

        [JsonProperty("SITHFIGHTER")]
        public Unit[] Sithfighter { get; set; }

        [JsonProperty("XWINGRESISTANCE")]
        public Unit[] Xwingresistance { get; set; }

        [JsonProperty("QIRA")]
        public Unit[] Qira { get; set; }

        [JsonProperty("POE")]
        public Unit[] Poe { get; set; }

        [JsonProperty("TALIA")]
        public Unit[] Talia { get; set; }

        [JsonProperty("NIGHTSISTERZOMBIE")]
        public Unit[] Nightsisterzombie { get; set; }

        [JsonProperty("SCARIFREBEL")]
        public Unit[] Scarifrebel { get; set; }

        [JsonProperty("GRANDADMIRALTHRAWN")]
        public Unit[] Grandadmiralthrawn { get; set; }

        [JsonProperty("JEDISTARFIGHTERANAKIN")]
        public Unit[] Jedistarfighteranakin { get; set; }

        [JsonProperty("IMPERIALPROBEDROID")]
        public Unit[] Imperialprobedroid { get; set; }

        [JsonProperty("SABINEWRENS3")]
        public Unit[] Sabinewrens3 { get; set; }

        [JsonProperty("PAPLOO")]
        public Unit[] Paploo { get; set; }

        [JsonProperty("YOUNGHAN")]
        public Unit[] Younghan { get; set; }

        [JsonProperty("EWOKELDER")]
        public Unit[] Ewokelder { get; set; }

        [JsonProperty("CHIEFCHIRPA")]
        public Unit[] Chiefchirpa { get; set; }

        [JsonProperty("CT5555")]
        public Unit[] Ct5555 { get; set; }

        [JsonProperty("VISASMARR")]
        public Unit[] Visasmarr { get; set; }

        [JsonProperty("LUKESKYWALKER")]
        public Unit[] Lukeskywalker { get; set; }

        [JsonProperty("KITFISTO")]
        public Unit[] Kitfisto { get; set; }

        [JsonProperty("HOUNDSTOOTH")]
        public Unit[] Houndstooth { get; set; }

        [JsonProperty("ADMINISTRATORLANDO")]
        public Unit[] Administratorlando { get; set; }

        [JsonProperty("ANAKINKNIGHT")]
        public Unit[] Anakinknight { get; set; }

        [JsonProperty("IG2000")]
        public Unit[] Ig2000 { get; set; }

        [JsonProperty("NIGHTSISTERSPIRIT")]
        public Unit[] Nightsisterspirit { get; set; }

        [JsonProperty("XANADUBLOOD")]
        public Unit[] Xanadublood { get; set; }

        [JsonProperty("CLONESERGEANTPHASEI")]
        public Unit[] Clonesergeantphasei { get; set; }

        [JsonProperty("ADMIRALACKBAR")]
        public Unit[] Admiralackbar { get; set; }

        [JsonProperty("FULCRUMAHSOKA")]
        public Unit[] Fulcrumahsoka { get; set; }

        [JsonProperty("STORMTROOPER")]
        public Unit[] Stormtrooper { get; set; }

        [JsonProperty("LUMINARAUNDULI")]
        public Unit[] Luminaraunduli { get; set; }

        [JsonProperty("JANGOFETT")]
        public Unit[] Jangofett { get; set; }

        [JsonProperty("HOTHREBELSCOUT")]
        public Unit[] Hothrebelscout { get; set; }

        [JsonProperty("GHOST")]
        public Unit[] Ghost { get; set; }

        [JsonProperty("NUTEGUNRAY")]
        public Unit[] Nutegunray { get; set; }

        [JsonProperty("SLAVE1")]
        public Unit[] Slave1 { get; set; }

        [JsonProperty("TIEADVANCED")]
        public Unit[] Tieadvanced { get; set; }

        [JsonProperty("VEERS")]
        public Unit[] Veers { get; set; }

        [JsonProperty("PAO")]
        public Unit[] Pao { get; set; }

        [JsonProperty("IG88")]
        public Unit[] Ig88 { get; set; }

        [JsonProperty("MILLENNIUMFALCONPRISTINE")]
        public Unit[] Millenniumfalconpristine { get; set; }

        [JsonProperty("EBONHAWK")]
        public Unit[] Ebonhawk { get; set; }

        [JsonProperty("REY")]
        public Unit[] Rey { get; set; }

        [JsonProperty("BLADEOFDORIN")]
        public Unit[] Bladeofdorin { get; set; }

        [JsonProperty("DARTHNIHILUS")]
        public Unit[] Darthnihilus { get; set; }

        [JsonProperty("EMPERORPALPATINE")]
        public Unit[] Emperorpalpatine { get; set; }

        [JsonProperty("PLOKOON")]
        public Unit[] Plokoon { get; set; }

        [JsonProperty("DAKA")]
        public Unit[] Daka { get; set; }

        [JsonProperty("GRANDMOFFTARKIN")]
        public Unit[] Grandmofftarkin { get; set; }

        [JsonProperty("LOGRAY")]
        public Unit[] Logray { get; set; }

        [JsonProperty("GEONOSIANSOLDIER")]
        public Unit[] Geonosiansoldier { get; set; }

        [JsonProperty("RESISTANCETROOPER")]
        public Unit[] Resistancetrooper { get; set; }

        [JsonProperty("HK47")]
        public Unit[] Hk47 { get; set; }

        [JsonProperty("TUSKENRAIDER")]
        public Unit[] Tuskenraider { get; set; }

        [JsonProperty("CASSIANANDOR")]
        public Unit[] Cassianandor { get; set; }

        [JsonProperty("BAZEMALBUS")]
        public Unit[] Bazemalbus { get; set; }

        [JsonProperty("ZAALBAR")]
        public Unit[] Zaalbar { get; set; }

        [JsonProperty("CLONEWARSCHEWBACCA")]
        public Unit[] Clonewarschewbacca { get; set; }

        [JsonProperty("COMMANDERLUKESKYWALKER")]
        public Unit[] Commanderlukeskywalker { get; set; }

        [JsonProperty("UWINGROGUEONE")]
        public Unit[] Uwingrogueone { get; set; }

        [JsonProperty("DARTHSIDIOUS")]
        public Unit[] Darthsidious { get; set; }

        [JsonProperty("OLDBENKENOBI")]
        public Unit[] Oldbenkenobi { get; set; }

        [JsonProperty("KYLORENUNMASKED")]
        public Unit[] Kylorenunmasked { get; set; }

        [JsonProperty("YOUNGCHEWBACCA")]
        public Unit[] Youngchewbacca { get; set; }

        [JsonProperty("YOUNGLANDO")]
        public Unit[] Younglando { get; set; }

        [JsonProperty("HANSOLO")]
        public Unit[] Hansolo { get; set; }

        [JsonProperty("SMUGGLERHAN")]
        public Unit[] Smugglerhan { get; set; }

        [JsonProperty("UGNAUGHT")]
        public Unit[] Ugnaught { get; set; }

        [JsonProperty("UMBARANSTARFIGHTER")]
        public Unit[] Umbaranstarfighter { get; set; }

        [JsonProperty("WAMPA")]
        public Unit[] Wampa { get; set; }

        [JsonProperty("SMUGGLERCHEWBACCA")]
        public Unit[] Smugglerchewbacca { get; set; }

        [JsonProperty("R2D2_LEGENDARY")]
        public Unit[] R2D2Legendary { get; set; }

        [JsonProperty("UWINGSCARIF")]
        public Unit[] Uwingscarif { get; set; }

        [JsonProperty("HUMANTHUG")]
        public Unit[] Humanthug { get; set; }

        [JsonProperty("GARSAXON")]
        public Unit[] Garsaxon { get; set; }

        [JsonProperty("CORUSCANTUNDERWORLDPOLICE")]
        public Unit[] Coruscantunderworldpolice { get; set; }

        [JsonProperty("ENFYSNEST")]
        public Unit[] Enfysnest { get; set; }

        [JsonProperty("MOTHERTALZIN")]
        public Unit[] Mothertalzin { get; set; }

        [JsonProperty("BIGGSDARKLIGHTER")]
        public Unit[] Biggsdarklighter { get; set; }

        [JsonProperty("CC2224")]
        public Unit[] Cc2224 { get; set; }

        [JsonProperty("JEDIKNIGHTREVAN")]
        public Unit[] Jediknightrevan { get; set; }

        [JsonProperty("NIGHTSISTERINITIATE")]
        public Unit[] Nightsisterinitiate { get; set; }

        [JsonProperty("CT7567")]
        public Unit[] Ct7567 { get; set; }

        [JsonProperty("EMPERORSSHUTTLE")]
        public Unit[] Emperorsshuttle { get; set; }

        [JsonProperty("ZAMWESELL")]
        public Unit[] Zamwesell { get; set; }

        [JsonProperty("CAPITALMONCALAMARICRUISER")]
        public Unit[] Capitalmoncalamaricruiser { get; set; }

        [JsonProperty("SUNFAC")]
        public Unit[] Sunfac { get; set; }

        [JsonProperty("EMBO")]
        public Unit[] Embo { get; set; }

        [JsonProperty("IMAGUNDI")]
        public Unit[] Imagundi { get; set; }

        [JsonProperty("TIESILENCER")]
        public Unit[] Tiesilencer { get; set; }

        [JsonProperty("XWINGRED3")]
        public Unit[] Xwingred3 { get; set; }

        [JsonProperty("EETHKOTH")]
        public Unit[] Eethkoth { get; set; }

        [JsonProperty("CAPITALSTARDESTROYER")]
        public Unit[] Capitalstardestroyer { get; set; }

        [JsonProperty("T3_M4")]
        public Unit[] T3M4 { get; set; }

        [JsonProperty("XWINGRED2")]
        public Unit[] Xwingred2 { get; set; }

        [JsonProperty("JEDISTARFIGHTERAHSOKATANO")]
        public Unit[] Jedistarfighterahsokatano { get; set; }

        [JsonProperty("HOTHREBELSOLDIER")]
        public Unit[] Hothrebelsoldier { get; set; }

        [JsonProperty("ASAJVENTRESS")]
        public Unit[] Asajventress { get; set; }

        [JsonProperty("GRIEVOUS")]
        public Unit[] Grievous { get; set; }

        [JsonProperty("CHIRRUTIMWE")]
        public Unit[] Chirrutimwe { get; set; }

        [JsonProperty("JUHANI")]
        public Unit[] Juhani { get; set; }

        [JsonProperty("KANANJARRUSS3")]
        public Unit[] Kananjarruss3 { get; set; }

        [JsonProperty("ROSETICO")]
        public Unit[] Rosetico { get; set; }

        [JsonProperty("JEDIKNIGHTCONSULAR")]
        public Unit[] Jediknightconsular { get; set; }

        [JsonProperty("AHSOKATANO")]
        public Unit[] Ahsokatano { get; set; }

        [JsonProperty("TIEFIGHTERIMPERIAL")]
        public Unit[] Tiefighterimperial { get; set; }

        [JsonProperty("STORMTROOPERHAN")]
        public Unit[] Stormtrooperhan { get; set; }

        [JsonProperty("HERASYNDULLAS3")]
        public Unit[] Herasyndullas3 { get; set; }

        [JsonProperty("BASTILASHANDARK")]
        public Unit[] Bastilashandark { get; set; }

        [JsonProperty("BOSSK")]
        public Unit[] Bossk { get; set; }

        [JsonProperty("FIRSTORDEREXECUTIONER")]
        public Unit[] Firstorderexecutioner { get; set; }

        [JsonProperty("VADER")]
        public Unit[] Vader { get; set; }

        [JsonProperty("COUNTDOOKU")]
        public Unit[] Countdooku { get; set; }

        [JsonProperty("MISSIONVAO")]
        public Unit[] Missionvao { get; set; }

        [JsonProperty("ARC170CLONESERGEANT")]
        public Unit[] Arc170Clonesergeant { get; set; }

        [JsonProperty("MAUL")]
        public Unit[] Maul { get; set; }

        [JsonProperty("JOLEEBINDO")]
        public Unit[] Joleebindo { get; set; }

        [JsonProperty("CHIEFNEBIT")]
        public Unit[] Chiefnebit { get; set; }

        [JsonProperty("TUSKENSHAMAN")]
        public Unit[] Tuskenshaman { get; set; }

        [JsonProperty("IMPERIALSUPERCOMMANDO")]
        public Unit[] Imperialsupercommando { get; set; }

        [JsonProperty("FIRSTORDERTIEPILOT")]
        public Unit[] Firstordertiepilot { get; set; }

        [JsonProperty("DARTHSION")]
        public Unit[] Darthsion { get; set; }

        [JsonProperty("CAPITALJEDICRUISER")]
        public Unit[] Capitaljedicruiser { get; set; }

        [JsonProperty("CAPITALCHIMAERA")]
        public Unit[] Capitalchimaera { get; set; }

        [JsonProperty("COMMANDSHUTTLE")]
        public Unit[] Commandshuttle { get; set; }

        [JsonProperty("FIRSTORDERTROOPER")]
        public Unit[] Firstordertrooper { get; set; }

        [JsonProperty("PHANTOM2")]
        public Unit[] Phantom2 { get; set; }

        [JsonProperty("HOTHLEIA")]
        public Unit[] Hothleia { get; set; }

        [JsonProperty("GREEDO")]
        public Unit[] Greedo { get; set; }

        [JsonProperty("BODHIROOK")]
        public Unit[] Bodhirook { get; set; }

        [JsonProperty("CHEWBACCALEGENDARY")]
        public Unit[] Chewbaccalegendary { get; set; }

        [JsonProperty("CHOPPERS3")]
        public Unit[] Choppers3 { get; set; }

        [JsonProperty("BOBAFETT")]
        public Unit[] Bobafett { get; set; }

        [JsonProperty("WEDGEANTILLES")]
        public Unit[] Wedgeantilles { get; set; }

        [JsonProperty("DEATHTROOPER")]
        public Unit[] Deathtrooper { get; set; }

        [JsonProperty("CARTHONASI")]
        public Unit[] Carthonasi { get; set; }

        [JsonProperty("WICKET")]
        public Unit[] Wicket { get; set; }

        [JsonProperty("GEONOSIANSTARFIGHTER2")]
        public Unit[] Geonosianstarfighter2 { get; set; }

        [JsonProperty("GEONOSIANSTARFIGHTER1")]
        public Unit[] Geonosianstarfighter1 { get; set; }

        [JsonProperty("GEONOSIANSTARFIGHTER3")]
        public Unit[] Geonosianstarfighter3 { get; set; }

        [JsonProperty("MACEWINDU")]
        public Unit[] Macewindu { get; set; }

        [JsonProperty("DATHCHA")]
        public Unit[] Dathcha { get; set; }

        [JsonProperty("JYNERSO")]
        public Unit[] Jynerso { get; set; }

        [JsonProperty("TEEBO")]
        public Unit[] Teebo { get; set; }

        [JsonProperty("XWINGBLACKONE")]
        public Unit[] Xwingblackone { get; set; }

        [JsonProperty("SITHBOMBER")]
        public Unit[] Sithbomber { get; set; }

        [JsonProperty("AMILYNHOLDO")]
        public Unit[] Amilynholdo { get; set; }

        [JsonProperty("GAUNTLETSTARFIGHTER")]
        public Unit[] Gauntletstarfighter { get; set; }

        [JsonProperty("SITHASSASSIN")]
        public Unit[] Sithassassin { get; set; }

        [JsonProperty("SNOWTROOPER")]
        public Unit[] Snowtrooper { get; set; }

        [JsonProperty("SHORETROOPER")]
        public Unit[] Shoretrooper { get; set; }

        [JsonProperty("PRINCESSLEIA")]
        public Unit[] Princessleia { get; set; }

        [JsonProperty("SAVAGEOPRESS")]
        public Unit[] Savageopress { get; set; }

        [JsonProperty("FINN")]
        public Unit[] Finn { get; set; }

        [JsonProperty("PHASMA")]
        public Unit[] Phasma { get; set; }

        [JsonProperty("EZRABRIDGERS3")]
        public Unit[] Ezrabridgers3 { get; set; }

        [JsonProperty("TIEREAPER")]
        public Unit[] Tiereaper { get; set; }

        [JsonProperty("JEDISTARFIGHTERCONSULAR")]
        public Unit[] Jedistarfighterconsular { get; set; }

        [JsonProperty("FIRSTORDEROFFICERMALE")]
        public Unit[] Firstorderofficermale { get; set; }

        [JsonProperty("RESISTANCEPILOT")]
        public Unit[] Resistancepilot { get; set; }

        [JsonProperty("AURRA_SING")]
        public Unit[] AurraSing { get; set; }

        [JsonProperty("JEDIKNIGHTGUARDIAN")]
        public Unit[] Jediknightguardian { get; set; }

        [JsonProperty("CANDEROUSORDO")]
        public Unit[] Canderousordo { get; set; }

        [JsonProperty("CADBANE")]
        public Unit[] Cadbane { get; set; }

        [JsonProperty("TIEFIGHTERPILOT")]
        public Unit[] Tiefighterpilot { get; set; }

        [JsonProperty("GRANDMASTERYODA")]
        public Unit[] Grandmasteryoda { get; set; }

        [JsonProperty("BISTAN")]
        public Unit[] Bistan { get; set; }
    }

    public partial class Unit
    {
        [JsonProperty("updated")]
        public long Updated { get; set; }

        [JsonProperty("player")]
        public string Player { get; set; }

        [JsonProperty("allyCode")]
        public long AllyCode { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("gp")]
        public long Gp { get; set; }

        [JsonProperty("starLevel")]
        public long StarLevel { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("gearLevel")]
        public long GearLevel { get; set; }

        [JsonProperty("gear")]
        public string[] Gear { get; set; }

        [JsonProperty("zetas")]
        public Zeta[] Zetas { get; set; }

        [JsonProperty("mods")]
        public PurpleMod[] Mods { get; set; }
    }

    public partial class PurpleMod
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("set", NullValueHandling = NullValueHandling.Ignore)]
        public long? Set { get; set; }

        [JsonProperty("level", NullValueHandling = NullValueHandling.Ignore)]
        public long? Level { get; set; }

        [JsonProperty("pips", NullValueHandling = NullValueHandling.Ignore)]
        public long? Pips { get; set; }

        [JsonProperty("tier", NullValueHandling = NullValueHandling.Ignore)]
        public long? Tier { get; set; }

        [JsonProperty("stat", NullValueHandling = NullValueHandling.Ignore)]
        public double[][] Stat { get; set; }
    }

    public partial class Zeta
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("tier")]
        public long Tier { get; set; }

        [JsonProperty("nameKey")]
        public string NameKey { get; set; }

        [JsonProperty("isZeta")]
        public bool IsZeta { get; set; }
    }

    public partial class Units
    {
        public static Units FromJson(string json) => JsonConvert.DeserializeObject<Units>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Units self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
