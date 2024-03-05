using System.ComponentModel.DataAnnotations;

namespace src.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Il campo Codice Utente è obbligatorio.")]
        public string CodUser { get; set; }

        [Required(ErrorMessage = "Il campo Nome Utente è obbligatorio.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Il campo Password è obbligatorio.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Il campo Nome è obbligatorio.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Il campo Cognome è obbligatorio.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Il campo Tipo è obbligatorio.")]
        public string Type { get; set; }
    }
}
