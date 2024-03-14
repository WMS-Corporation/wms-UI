using Newtonsoft.Json;
using src.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace src.Services
{
    public class TaskService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _webServiceBaseUrl;

        public TaskService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, string webServiceBaseUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _webServiceBaseUrl = webServiceBaseUrl ?? throw new ArgumentNullException(nameof(webServiceBaseUrl));
        }

        private string GetAccessToken()
        {
            return _httpContextAccessor.HttpContext.Session.GetString(Constants.AuthToken);
        }

        private void AddAccessTokenToRequest(HttpRequestMessage request)
        {
            var token = GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task AddTaskAsync(TaskModel task)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_webServiceBaseUrl}/api/tasks");
            AddAccessTokenToRequest(request);

            request.Content = new StringContent(JsonConvert.SerializeObject(task), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateTaskAsync(TaskModel updatedTask)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_webServiceBaseUrl}/api/tasks/{updatedTask.Id}");
            AddAccessTokenToRequest(request);

            request.Content = new StringContent(JsonConvert.SerializeObject(updatedTask), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTaskAsync(string taskId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_webServiceBaseUrl}/api/tasks/{taskId}");
            AddAccessTokenToRequest(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<TaskModel> GetTaskAsync(string taskId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_webServiceBaseUrl}/api/tasks/{taskId}");
            AddAccessTokenToRequest(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TaskModel>(jsonContent);
        }

        public async Task<List<TaskModel>> GetTasksAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_webServiceBaseUrl}/api/tasks/all");
            AddAccessTokenToRequest(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TaskModel>>(jsonContent);
        }
    }
}
