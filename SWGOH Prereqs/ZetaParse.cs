using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zetas
{
    public partial class Zetas
    {
        [JsonProperty("request")]
        public object Request { get; set; }

        [JsonProperty("sort")]
        public object Sort { get; set; }

        [JsonProperty("zetas")]
        public Zeta[] ZetasZetas { get; set; }

        [JsonProperty("details")]
        public Detail[] Details { get; set; }

        [JsonProperty("usage")]
        public Usage[] Usage { get; set; }

        [JsonProperty("credits")]
        public Credit[] Credits { get; set; }

        [JsonProperty("updated")]
        public long Updated { get; set; }
    }

    public partial class Credit
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Discord")]
        public string Discord { get; set; }

        [JsonProperty("Link")]
        public string Link { get; set; }

        [JsonProperty("Credit")]
        public string CreditCredit { get; set; }
    }

    public partial class Detail
    {
        [JsonProperty("Faction")]
        public string Faction { get; set; }

        [JsonProperty("Toon")]
        public string Toon { get; set; }

        [JsonProperty("Type")]
        public TypeEnum Type { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("MinLevel")]
        public long MinLevel { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }
    }

    public partial class Usage
    {
        [JsonProperty("version")]
        public string[] Version { get; set; }

        [JsonProperty("url")]
        public Uri[] Url { get; set; }

        [JsonProperty("params")]
        public Params Params { get; set; }

        [JsonProperty("request")]
        public Request Request { get; set; }

        [JsonProperty("sort")]
        public Sort Sort { get; set; }

        [JsonProperty("examples")]
        public string[] Examples { get; set; }

        [JsonProperty("note")]
        public string[] Note { get; set; }
    }

    public partial class Params
    {
        [JsonProperty("request")]
        public string Request { get; set; }

        [JsonProperty("sort")]
        public string Sort { get; set; }
    }

    public partial class Request
    {
        [JsonProperty("zetas")]
        public string Zetas { get; set; }

        [JsonProperty("usage")]
        public string Usage { get; set; }

        [JsonProperty("credits")]
        public string Credits { get; set; }

        [JsonProperty("null")]
        public string Null { get; set; }
    }

    public partial class Sort
    {
        [JsonProperty("pvp")]
        public string Pvp { get; set; }

        [JsonProperty("tw")]
        public string Tw { get; set; }

        [JsonProperty("tb")]
        public string Tb { get; set; }

        [JsonProperty("pit")]
        public string Pit { get; set; }

        [JsonProperty("tank")]
        public string Tank { get; set; }

        [JsonProperty("sith")]
        public string Sith { get; set; }

        [JsonProperty("versatility")]
        public string Versatility { get; set; }
    }

    public partial class Zeta
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public TypeEnum Type { get; set; }

        [JsonProperty("toon")]
        public string Toon { get; set; }

        [JsonProperty("pvp")]
        public double Pvp { get; set; }

        [JsonProperty("tw")]
        public double Tw { get; set; }

        [JsonProperty("tb")]
        public double Tb { get; set; }

        [JsonProperty("pit")]
        public double Pit { get; set; }

        [JsonProperty("tank")]
        public double Tank { get; set; }

        [JsonProperty("sith")]
        public double Sith { get; set; }

        [JsonProperty("versa")]
        public double Versa { get; set; }
    }

    public enum TypeEnum { Leader, Special, Unique };

    public partial class Zetas
    {
        public static Zetas FromJson(string json) => JsonConvert.DeserializeObject<Zetas>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Zetas self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                TypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Leader":
                    return TypeEnum.Leader;
                case "Special":
                    return TypeEnum.Special;
                case "Unique":
                    return TypeEnum.Unique;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            switch (value)
            {
                case TypeEnum.Leader:
                    serializer.Serialize(writer, "Leader");
                    return;
                case TypeEnum.Special:
                    serializer.Serialize(writer, "Special");
                    return;
                case TypeEnum.Unique:
                    serializer.Serialize(writer, "Unique");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }
}
