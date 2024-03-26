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
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _webServiceBaseUrl;

        public ProductService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, string webServiceBaseUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _webServiceBaseUrl = webServiceBaseUrl ?? throw new ArgumentNullException(nameof(webServiceBaseUrl));
        }

        private async Task<string> HttpProductMicroserviceOperationAsync(string relativeExternalSubRoute, HttpMethod httpMethod, string serializedContent)
        {
            var relativeUrl = !string.IsNullOrEmpty(relativeExternalSubRoute) ? $"/{relativeExternalSubRoute}" : string.Empty;
            return await Utils.HttpUtils.ExternaMicroserviceHttpAsyncOperation($"{Constants.ProductMicroserviceRouteName}{relativeUrl}", httpMethod, serializedContent, _webServiceBaseUrl, _httpContextAccessor, _httpClient);
        }

        public async Task<List<ProductModel>> GetProductsAsync()
        {
            var serializedResponse = await HttpProductMicroserviceOperationAsync("all", HttpMethod.Get, null);
            return JsonConvert.DeserializeObject<List<ProductModel>>(serializedResponse);
        }

        public async Task<ProductModel> GetProductAsync(string codProduct)
        {
            var serializedResponse = await HttpProductMicroserviceOperationAsync(codProduct, HttpMethod.Get, null);
            return JsonConvert.DeserializeObject<ProductModel>(serializedResponse);
        }

        public async Task AddProductAsync(ProductModel product)
        {
            var serializedProduct = JsonConvert.SerializeObject(product);
            await HttpProductMicroserviceOperationAsync("create", HttpMethod.Post, serializedProduct);
        }

        public async Task UpdateProductAsync(string codProduct, ProductModel updatedProduct)
        {
            var serializedProduct = JsonConvert.SerializeObject(updatedProduct);
            await HttpProductMicroserviceOperationAsync(codProduct, HttpMethod.Put, serializedProduct);
        }

        public async Task DeleteProductAsync(string codProduct)
        {
            await HttpProductMicroserviceOperationAsync(codProduct, HttpMethod.Delete, null);
        }
    }
}
