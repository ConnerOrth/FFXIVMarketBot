using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FFXIVMarketBot.Models
{
    public partial class ItemListings
    {
        [JsonProperty("dcName")]
        public string DcName { get; set; }

        [JsonProperty("itemID")]
        public long ItemId { get; set; }

        [JsonProperty("lastUploadTime")]
        public long LastUploadTime { get; set; }

        [JsonProperty("listings")]
        public List<Listing> Listings { get; set; }

        [JsonProperty("recentHistory")]
        public List<RecentHistory> RecentHistory { get; set; }

        [JsonProperty("averagePrice")]
        public double AveragePrice { get; set; }

        [JsonProperty("averagePriceNQ")]
        public long AveragePriceNq { get; set; }

        [JsonProperty("averagePriceHQ")]
        public double AveragePriceHq { get; set; }

        [JsonProperty("saleVelocity")]
        public double SaleVelocity { get; set; }

        [JsonProperty("saleVelocityNQ")]
        public double SaleVelocityNq { get; set; }

        [JsonProperty("saleVelocityHQ")]
        public double SaleVelocityHq { get; set; }

        [JsonProperty("saleVelocityUnits")]
        public string SaleVelocityUnits { get; set; }

        [JsonProperty("stackSizeHistogram")]
        public Dictionary<string, long> StackSizeHistogram { get; set; }

        [JsonProperty("stackSizeHistogramNQ")]
        public Dictionary<string, long> StackSizeHistogramNq { get; set; }

        [JsonProperty("stackSizeHistogramHQ")]
        public Dictionary<string, long> StackSizeHistogramHq { get; set; }
    }

    public partial class Listing
    {
        [JsonProperty("creatorID")]
        public string CreatorId { get; set; }

        [JsonProperty("creatorName")]
        public string CreatorName { get; set; }

        [JsonProperty("hq")]
        public bool Hq { get; set; }

        [JsonProperty("isCrafted")]
        public bool IsCrafted { get; set; }

        [JsonProperty("lastReviewTime")]
        public long LastReviewTime { get; set; }

        [JsonProperty("listingID")]
        public string ListingId { get; set; }

        [JsonProperty("materia")]
        public List<object> Materia { get; set; }

        [JsonProperty("onMannequin")]
        public bool OnMannequin { get; set; }

        [JsonProperty("pricePerUnit")]
        public long PricePerUnit { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("retainerCity")]
        public long RetainerCity { get; set; }

        public string RetainerCityName => RetainerCityConverter.Cities[RetainerCity];

        [JsonProperty("retainerID")]
        public string RetainerId { get; set; }

        [JsonProperty("retainerName")]
        public string RetainerName { get; set; }

        [JsonProperty("sellerID")]
        public string SellerId { get; set; }

        [JsonProperty("stainID")]
        public long StainId { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("worldName")]
        public string WorldName { get; set; }

        static class RetainerCityConverter
        {
            public static IDictionary<long, string> Cities = new Dictionary<long, string>()
        {
            { 1, "Limsa Lominsa" },
            { 2, "Gridania" },
            { 3, "Ul'dah" },
            { 4, "Ishgard" },
            { 7, "Kugane" },
            { 10, "Crystarium" }
        };
        }
    }

    public partial class RecentHistory
    {
        [JsonProperty("buyerName")]
        public string BuyerName { get; set; }

        [JsonProperty("hq")]
        public bool Hq { get; set; }

        [JsonProperty("pricePerUnit")]
        public long PricePerUnit { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("worldName")]
        public string WorldName { get; set; }
    }

    public partial class ItemListings
    {
        public static ItemListings FromJson(string json) => JsonConvert.DeserializeObject<ItemListings>(json, Converter.Settings);
    }
}
