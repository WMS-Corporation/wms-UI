using Newtonsoft.Json;

namespace src.Models
{
    public class OrderModel
    {
        [JsonProperty("_date")]
        public DateTime Date { get; set; }

        [JsonProperty("_status")]
        public string Status { get; set; }

        [JsonProperty("_productCodeList")]
        public List<string> ProductCodeList { get; set; }

        [JsonProperty("_codOrder")]
        public string CodOrder { get; set; }
    }
}
