using Newtonsoft.Json;
using src.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace src.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _webServiceBaseUrl;

        public UserService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, string webServiceBaseUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _webServiceBaseUrl = webServiceBaseUrl ?? throw new ArgumentNullException(nameof(webServiceBaseUrl));
        }

        private string GetAccessToken()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("AuthToken");
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            var token = GetAccessToken();

            // Crea una richiesta HTTP GET con il token di autorizzazione
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_webServiceBaseUrl}/api/users/all");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Invia la richiesta e verifica il successo
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Leggi e deserializza il JSON dalla risposta
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<UserModel>>(jsonContent);
        }
        public async Task<UserModel> GetUserAsync(string codUser)
        {
            var token = GetAccessToken();

            // Crea una richiesta HTTP GET con il token di autorizzazione
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_webServiceBaseUrl}/api/users/{codUser}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Invia la richiesta e verifica il successo
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Leggi e deserializza il JSON dalla risposta
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserModel>(jsonContent);
        }

        public async Task AddUserAsync(UserModel user)
        {
            var token = GetAccessToken();

            // Crea una richiesta HTTP POST con il token di autorizzazione
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_webServiceBaseUrl}/api/users");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Aggiungi il corpo della richiesta come JSON
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            // Invia la richiesta e verifica il successo
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateUserAsync(int userId, UserModel updatedUser)
        {
            var token = GetAccessToken();

            // Crea una richiesta HTTP PUT con il token di autorizzazione
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_webServiceBaseUrl}/api/users/{userId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Aggiungi il corpo della richiesta come JSON
            request.Content = new StringContent(JsonConvert.SerializeObject(updatedUser), Encoding.UTF8, "application/json");

            // Invia la richiesta e verifica il successo
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var token = GetAccessToken();

            // Crea una richiesta HTTP DELETE con il token di autorizzazione
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_webServiceBaseUrl}/api/users/{userId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Invia la richiesta e verifica il successo
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<LoginResponse> LoginAsync(LoginModel loginModel)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_webServiceBaseUrl}/api/users/login");

            // Aggiungi il corpo della richiesta come JSON
            request.Content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        public async Task RegisterAsync(RegisterModel registerModel)
        {
            var token = GetAccessToken();

            // Crea una richiesta HTTP POST con il token di autorizzazione
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_webServiceBaseUrl}/api/users/register");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Aggiungi il corpo della richiesta come JSON
            request.Content = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");

            // Invia la richiesta e verifica il successo
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

    }
}
