using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace src.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Il campo {0} è obbligatorio.")]
        [JsonProperty("_username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Il campo {0} è obbligatorio.")]
        [JsonProperty("_password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Il campo {0} è obbligatorio.")]
        [JsonProperty("_name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Il campo {0} è obbligatorio.")]
        [JsonProperty("_surname")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Il campo {0} è obbligatorio.")]
        [JsonProperty("_type")]
        public string Type { get; } = "Operational";
    }
}
