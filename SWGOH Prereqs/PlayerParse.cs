using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PlayerParse
{
    public partial class Player
    {
        [JsonProperty("players")]
        public PlayerElement[] PlayerList { get; set; }
    }

    public partial class PlayerElement
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("allyCode")]
        public long AllyCode { get; set; }

        [JsonProperty("stats")]
        public Stat[] Stats { get; set; }

        [JsonProperty("roster")]
        public Roster[] Roster { get; set; }

        [JsonProperty("updated")]
        public long Updated { get; set; }

        [JsonProperty("grandArena")]
        public GrandArena[] GrandArena { get; set; }

        [JsonProperty("grandArenaLifeTime")]
        public long GrandArenaLifeTime { get; set; }

        [JsonProperty("guildName")]
        public string GuildName { get; set; }
        [JsonProperty("guildRefId")]
        public string GuildRefId { get; set; }
        /* [JsonProperty("titles")]
         public Portraits Titles { get; set; }

         

         

         [JsonProperty("guildBannerColor")]
         public string GuildBannerColor { get; set; }

         [JsonProperty("guildBannerLogo")]
         public string GuildBannerLogo { get; set; }

         [JsonProperty("guildTypeId")]
         public string GuildTypeId { get; set; }

         [JsonProperty("portraits")]
         public Portraits Portraits { get; set; }
         [JsonProperty("arena")]
         public Arena Arena { get; set; }
         [JsonProperty("lastActivity")]
         public long LastActivity { get; set; }

         [JsonProperty("poUTCOffsetMinutes")]
         public long PoUtcOffsetMinutes { get; set; }
         */
    }

    #region arenas
    /*
    public partial class Arena
    {
        [JsonProperty("char")]
        public Char Char { get; set; }

        [JsonProperty("ship")]
        public Char Ship { get; set; }
    }

    public partial class Char
    {
        [JsonProperty("rank")]
        public long Rank { get; set; }

        [JsonProperty("squad")]
        public Squad[] Squad { get; set; }
    }
   public partial class Squad
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("defId")]
        public string DefId { get; set; }

        [JsonProperty("squadUnitType")]
        public long SquadUnitType { get; set; }
    }
    */
    #endregion
    public partial class GrandArena
    {
        [JsonProperty("seasonId")]
        public string SeasonId { get; set; }

        [JsonProperty("eventInstanceId")]
        public string EventInstanceId { get; set; }

        [JsonProperty("league")]
        public string League { get; set; }

        [JsonProperty("wins")]
        public long Wins { get; set; }

        [JsonProperty("losses")]
        public long Losses { get; set; }

        [JsonProperty("eliteDivision")]
        public bool EliteDivision { get; set; }

        [JsonProperty("seasonPoints")]
        public long SeasonPoints { get; set; }

        [JsonProperty("division")]
        public long Division { get; set; }
        /*
        [JsonProperty("joinTime")]
        public long JoinTime { get; set; }

        [JsonProperty("endTime")]
        public long EndTime { get; set; }

        [JsonProperty("remove")]
        public bool Remove { get; set; }
        */
        [JsonProperty("rank")]
        public long Rank { get; set; }
    }
    #region portraits
    /* public partial class Portraits
      {
          [JsonProperty("selected")]
          public string Selected { get; set; }

          [JsonProperty("unlocked")]
          public string[] Unlocked { get; set; }
      }
      */
    #endregion
    public partial class Roster
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("defId")]
        public string DefId { get; set; }

        [JsonProperty("rarity")]
        public long Rarity { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("gear")]
        public long Gear { get; set; }

        [JsonProperty("equipped")]
        public Equipped[] Equipped { get; set; }

        [JsonProperty("combatType")]
        public long CombatType { get; set; }

        [JsonProperty("skills")]
        public Skill[] Skills { get; set; }

        [JsonProperty("mods")]
        public Mod[] Mods { get; set; }

        [JsonProperty("gp")]
        public long Gp { get; set; }

        [JsonProperty("primaryUnitStat")]
        public object PrimaryUnitStat { get; set; }

        [JsonProperty("relic")]
        public Relic Relic { get; set; }

        [JsonProperty("stats", NullValueHandling = NullValueHandling.Ignore)]
        public Stats Stats { get; set; }
        /*[JsonProperty("xp")]
        public long Xp { get; set; }
        [JsonProperty("crew")]
        public Crew[] Crew { get; set; }
        [JsonProperty("nameKey")]
        public string NameKey { get; set; }*/
    }

    public partial class Stats
    {
        [JsonProperty("final")]
        public Dictionary<string, double> Final { get; set; }

        [JsonProperty("mods")]
        public Dictionary<string, double> Mods { get; set; }
    }
    #region crew
    /*
    public partial class Crew
    {
        [JsonProperty("unitId")]
        public string UnitId { get; set; }

        [JsonProperty("slot")]
        public long Slot { get; set; }

        [JsonProperty("skillReferenceList")]
        public SkillReferenceList[] SkillReferenceList { get; set; }

        [JsonProperty("skilllessCrewAbilityId")]
        public string SkilllessCrewAbilityId { get; set; }

        [JsonProperty("gp")]
        public long Gp { get; set; }

        [JsonProperty("cp")]
        public double Cp { get; set; }
    }
   
    public partial class SkillReferenceList
    {
        [JsonProperty("skillId")]
        public string SkillId { get; set; }

        [JsonProperty("requiredTier")]
        public long RequiredTier { get; set; }

        [JsonProperty("requiredRarity")]
        public long RequiredRarity { get; set; }

        [JsonProperty("requiredRelicTier")]
        public long RequiredRelicTier { get; set; }
    }
     */
    #endregion
    public partial class Equipped
    {
        [JsonProperty("equipmentId")]
        public string EquipmentId { get; set; }

        [JsonProperty("slot")]
        public long Slot { get; set; }

        [JsonProperty("nameKey")]
        public string NameKey { get; set; }
    }

    public partial class Mod
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("tier")]
        public long Tier { get; set; }

        [JsonProperty("slot")]
        public long Slot { get; set; }

        [JsonProperty("set")]
        public long Set { get; set; }

        [JsonProperty("pips")]
        public long Pips { get; set; }

        [JsonProperty("primaryStat")]
        public PrimaryStat PrimaryStat { get; set; }

        [JsonProperty("secondaryStat")]
        public SecondaryStat[] SecondaryStat { get; set; }
    }

    public partial class PrimaryStat
    {
        [JsonProperty("unitStat")]
        public long UnitStat { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }

    public partial class SecondaryStat
    {
        [JsonProperty("unitStat")]
        public long UnitStat { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("roll")]
        public long Roll { get; set; }
    }

    public partial class Relic
    {
        [JsonProperty("currentTier")]
        public long CurrentTier { get; set; }
    }

    public partial class Skill
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("tier")]
        public long Tier { get; set; }

        [JsonProperty("nameKey")]
        public string NameKey { get; set; }

        [JsonProperty("isZeta")]
        public bool IsZeta { get; set; }

        [JsonProperty("tiers")]
        public long Tiers { get; set; }
    }

    public partial class Stat
    {
        [JsonProperty("nameKey")]
        public string NameKey { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }

        [JsonProperty("index")]
        public long Index { get; set; }
    }

    public partial class Player
    {
        public static Player FromJson(string json) => JsonConvert.DeserializeObject<Player>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Player self) => JsonConvert.SerializeObject(self, Converter.Settings);
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