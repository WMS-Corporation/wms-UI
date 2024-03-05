using Newtonsoft.Json;

namespace src.Models
{
    public class UserModel
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_codUser")]
        public string CodUser { get; set; }

        [JsonProperty("_username")]
        public string Username { get; set; }

        [JsonProperty("_password")]
        public string Password { get; set; }

        [JsonProperty("_name")]
        public string Name { get; set; }

        [JsonProperty("_surname")]
        public string Surname { get; set; }

        [JsonProperty("_type")]
        public string Type { get; set; }
    }
}
