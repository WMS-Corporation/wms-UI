using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace src.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Il campo Nome Utente è obbligatorio.")]
        [JsonProperty("_username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Il campo Password è obbligatorio.")]
        [JsonProperty("_password")]
        public string Password { get; set; }
    }
}
