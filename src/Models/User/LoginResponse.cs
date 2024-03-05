namespace src.Models
{
    public class LoginResponse
    {
        public string Message { get; set; }
        public UserModel User { get; set; }
        public string Token { get; set; }
    }
}
