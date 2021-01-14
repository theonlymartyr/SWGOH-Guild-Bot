using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Users
{
    public partial class Users
    {
        [JsonProperty("Users")]
        public User[] PlayerList { get; set; }
    }

    public partial class User
    {
        [JsonProperty("discord")]
        public string Id { get; set; }

        [JsonProperty("allycode")]
        public uint allycode { get; set; }
    }


    public partial class Users
    {
        public static Users FromJson(string json) => JsonConvert.DeserializeObject<Users>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Users self) => JsonConvert.SerializeObject(self, Converter.Settings);
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
