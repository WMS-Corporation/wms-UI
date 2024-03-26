using Newtonsoft.Json;

namespace src.Models
{
    public class ProductModel
    {
        [JsonProperty("_codProduct")]
        public string CodProduct { get; set; }

        [JsonProperty("_name")]
        public string Name { get; set; }

        [JsonProperty("_category")]
        public string Category { get; set; }

        [JsonProperty("_expirationDate")]
        public DateTime ExpirationDate { get; set; }

        [JsonProperty("_type")]
        public string Type { get; set; }
    }
}
