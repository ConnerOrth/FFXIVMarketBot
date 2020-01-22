using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FFXIVMarketBot.Models
{
    public partial class ItemSearch
    {
        [JsonProperty("Pagination")]
        public Pagination Pagination { get; set; }

        [JsonProperty("Results")]
        public List<Item> Items { get; set; }

        [JsonProperty("SpeedMs")]
        public long SpeedMs { get; set; }
    }

    public partial class Pagination
    {
        [JsonProperty("Page")]
        public long Page { get; set; }

        [JsonProperty("PageNext")]
        public object PageNext { get; set; }

        [JsonProperty("PagePrev")]
        public object PagePrev { get; set; }

        [JsonProperty("PageTotal")]
        public long PageTotal { get; set; }

        [JsonProperty("Results")]
        public long Results { get; set; }

        [JsonProperty("ResultsPerPage")]
        public long ResultsPerPage { get; set; }

        [JsonProperty("ResultsTotal")]
        public long ResultsTotal { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("ID")]
        public long Id { get; set; }

        [JsonProperty("Icon")]
        public string Icon { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("UrlType")]
        public string UrlType { get; set; }

        [JsonProperty("_")]
        public string Empty { get; set; }

        [JsonProperty("_Score")]
        public long Score { get; set; }
    }

    public partial class ItemSearch
    {
        public static ItemSearch FromJson(string json) => JsonConvert.DeserializeObject<ItemSearch>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ItemSearch self) => JsonConvert.SerializeObject(self, Converter.Settings);
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
