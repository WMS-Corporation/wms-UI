using Newtonsoft.Json;

namespace src.Models
{
    public class TaskModel
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_codOperator")]
        public string CodOperator { get; set; }

        [JsonProperty("_date")]
        public DateTime Date { get; set; }

        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("_status")]
        public string Status { get; set; }

        [JsonProperty("_productCodeList")]
        public List<string> ProductCodeList { get; set; }

        [JsonProperty("_codTask")]
        public string CodTask { get; set; }
    }
}
