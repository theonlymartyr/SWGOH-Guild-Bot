using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SWGOH_Prereqs
{
    public partial class SwgohPlayer
    {
        /* [JsonProperty("units")]
         public Unit[] Units { get; set; }
         */
        [JsonProperty("data")]
        public SwgohPlayerData Data { get; set; }
    }

    public partial class SwgohPlayerData
    {
        [JsonProperty("last_updated")]
        public DateTimeOffset LastUpdated { get; set; }

        [JsonProperty("galactic_power")]
        public long GalacticPower { get; set; }

        [JsonProperty("guild_id")]
        public long GuildId { get; set; }
        /*[JsonProperty("pve_battles_won")]
        public long PveBattlesWon { get; set; }

        [JsonProperty("character_galactic_power")]
        public long CharacterGalacticPower { get; set; }

        [JsonProperty("guild_contribution")]
        public long GuildContribution { get; set; }

        [JsonProperty("guild_exchange_donations")]
        public long GuildExchangeDonations { get; set; }

        [JsonProperty("fleet_arena")]
        public Arena FleetArena { get; set; }

        

        [JsonProperty("arena_leader_base_id")]
        public string ArenaLeaderBaseId { get; set; }

        [JsonProperty("galactic_war_won")]
        public long GalacticWarWon { get; set; }

        [JsonProperty("pve_hard_won")]
        public long PveHardWon { get; set; }
        */
        [JsonProperty("guild_name")]
        public string GuildName { get; set; }
        /*
        [JsonProperty("arena_rank")]
        public long ArenaRank { get; set; }

        [JsonProperty("guild_raid_won")]
        public long GuildRaidWon { get; set; }

        [JsonProperty("arena")]
        public Arena Arena { get; set; }
        */
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ally_code")]
        public long AllyCode { get; set; }
        /*
                [JsonProperty("pvp_battles_won")]
                public long PvpBattlesWon { get; set; }

                [JsonProperty("level")]
                public long Level { get; set; }

                [JsonProperty("ship_galactic_power")]
                public long ShipGalacticPower { get; set; }
                [JsonProperty("ship_battles_won")]
                public long ShipBattlesWon { get; set; }
                */
        [JsonProperty("url")]
        public string Url { get; set; }
    }
    /*
    public partial class Arena
    {
        [JsonProperty("members")]
        public string[] Members { get; set; }

        [JsonProperty("leader")]
        public string Leader { get; set; }

        [JsonProperty("rank")]
        public long Rank { get; set; }

        [JsonProperty("reinforcements", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Reinforcements { get; set; }
    }
    
    public partial class Unit
    {
        [JsonProperty("data")]
        public UnitData Data { get; set; }
    }

    public partial class UnitData
    {
        [JsonProperty("relic_tier")]
        public long RelicTier { get; set; }

        [JsonProperty("gear")]
        public Gear[] Gear { get; set; }

        [JsonProperty("power")]
        public long Power { get; set; }

        [JsonProperty("combat_type")]
        public long CombatType { get; set; }

        [JsonProperty("mod_set_ids")]
        public long[] ModSetIds { get; set; }

        [JsonProperty("base_id")]
        public string BaseId { get; set; }

        [JsonProperty("gear_level")]
        public long GearLevel { get; set; }

        [JsonProperty("stats")]
        public Dictionary<string, double> Stats { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("rarity")]
        public long Rarity { get; set; }

        [JsonProperty("ability_data")]
        public AbilityDatum[] AbilityData { get; set; }

        [JsonProperty("zeta_abilities")]
        public string[] ZetaAbilities { get; set; }
    }

    public partial class AbilityDatum
    {
        [JsonProperty("is_omega")]
        public bool IsOmega { get; set; }

        [JsonProperty("is_zeta")]
        public bool IsZeta { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ability_tier")]
        public long AbilityTier { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("tier_max")]
        public long TierMax { get; set; }
    }

    public partial class Gear
    {
        [JsonProperty("slot")]
        public long Slot { get; set; }

        [JsonProperty("is_obtained")]
        public bool IsObtained { get; set; }

        [JsonProperty("base_id")]
        public string BaseId { get; set; }
    }
    */
    public partial class SwgohPlayer
    {
        public static SwgohPlayer FromJson(string json) => JsonConvert.DeserializeObject<SwgohPlayer>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SwgohPlayer self) => JsonConvert.SerializeObject(self, Converter.Settings);
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
