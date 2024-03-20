using Newtonsoft.Json;
using src.Models;
using System.Net.Http;
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

        private async Task<string> HttpUsersMicroservicOperationAsync(string relativeExternalSubRoute, HttpMethod httpMethod, string serializedContent)
        {
            var relativeUrl = !string.IsNullOrEmpty(relativeExternalSubRoute) ? $"/{relativeExternalSubRoute}" : string.Empty;
            return await Utils.HttpUtils.ExternaMicroserviceHttpAsyncOperation($"{Constants.UsersMicroserviceRouteName}{relativeUrl}", httpMethod, serializedContent, _webServiceBaseUrl, _httpContextAccessor, _httpClient);
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            return JsonConvert.DeserializeObject<List<UserModel>>(await HttpUsersMicroservicOperationAsync("all", HttpMethod.Get, null));
        }

        public async Task<UserModel> GetUserAsync(string codUser)
        {
            return JsonConvert.DeserializeObject<UserModel>(await HttpUsersMicroservicOperationAsync(codUser, HttpMethod.Get, null));
        }

        public async Task AddUserAsync(UserModel user)
        {
            await HttpUsersMicroservicOperationAsync(null, HttpMethod.Post, JsonConvert.SerializeObject(user));
        }

        public async Task UpdateUserAsync(int userId, UserModel updatedUser)
        {
            await HttpUsersMicroservicOperationAsync(userId.ToString(), HttpMethod.Put, JsonConvert.SerializeObject(updatedUser));
        }

        public async Task DeleteUserAsync(int userId)
        {
            await HttpUsersMicroservicOperationAsync(userId.ToString(), HttpMethod.Delete, null);
        }

        #region Account
        public async Task<LoginResponse> LoginAsync(LoginModel loginModel)
        {
            return JsonConvert.DeserializeObject<LoginResponse>(await HttpUsersMicroservicOperationAsync("login", HttpMethod.Post, JsonConvert.SerializeObject(loginModel)));
        }

        public async Task RegisterAsync(RegisterModel registerModel)
        {
            await HttpUsersMicroservicOperationAsync("register", HttpMethod.Post, JsonConvert.SerializeObject(registerModel));
        }
        #endregion
    }
}
