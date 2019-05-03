using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GuildParse
{


    public partial class Guild
    {
        [JsonProperty("guild")]
        public GuildMember[] guild { get; set; }
    }

    public partial class GuildMember
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("members")]
        public long Members { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("required")]
        public long GuildRequired { get; set; }

        [JsonProperty("bannerColor")]
        public string BannerColor { get; set; }

        [JsonProperty("bannerLogo")]
        public string BannerLogo { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("gp")]
        public long Gp { get; set; }

        [JsonProperty("raid")]
        public Raid Raid { get; set; }

        [JsonProperty("roster")]
        public Roster[] Roster { get; set; }

        [JsonProperty("updated")]
        public long Updated { get; set; }
    }

    public partial class Raid
    {
        [JsonProperty("rancor")]
        public string Rancor { get; set; }

        [JsonProperty("aat")]
        public string Aat { get; set; }

        [JsonProperty("sith_raid")]
        public string SithRaid { get; set; }
    }

    public partial class Roster
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("guildMemberLevel")]
        public long GuildMemberLevel { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("allyCode")]
        public uint AllyCode { get; set; }

        [JsonProperty("gp")]
        public long Gp { get; set; }

        [JsonProperty("gpChar")]
        public long GpChar { get; set; }

        [JsonProperty("gpShip")]
        public long GpShip { get; set; }

        [JsonProperty("updated")]
        public long Updated { get; set; }
    }

    public partial class Guild
    {
        public static Guild FromJson(string json) => JsonConvert.DeserializeObject<Guild>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Guild self) => JsonConvert.SerializeObject(self, Converter.Settings);
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
