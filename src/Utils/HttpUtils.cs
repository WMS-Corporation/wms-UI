using Newtonsoft.Json;
using src.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace src.Utils
{
    public static class HttpUtils
    {
        public static void AddAccessTokenToRequest(IHttpContextAccessor httpContextAccessor, HttpRequestMessage request)
        {
            var token = httpContextAccessor.HttpContext.Session.GetString(Constants.AuthToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static async Task<string> ExternaMicroserviceHttpAsyncOperation(string relativeExternalSubRoute, HttpMethod httpMethod, string serializedContent, string webServiceBaseUrl, IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            var request = new HttpRequestMessage(httpMethod, $"{webServiceBaseUrl}/api/{relativeExternalSubRoute}");
            Utils.HttpUtils.AddAccessTokenToRequest(httpContextAccessor, request);

            if (!string.IsNullOrEmpty(serializedContent))
            {
                request.Content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
            }

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            return jsonContent;
        }
    }
}
