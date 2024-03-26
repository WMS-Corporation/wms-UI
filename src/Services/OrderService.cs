using AutoMapper;
using Newtonsoft.Json;
using src.Models;
using src.ViewModel;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace src.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _webServiceBaseUrl;
        private readonly IMapper _mapper;

        public OrderService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, string webServiceBaseUrl, IMapper mapper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _webServiceBaseUrl = webServiceBaseUrl ?? throw new ArgumentNullException(nameof(webServiceBaseUrl));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public OrderViewModel MapOrderModelToViewModel(ProductService productService, OrderModel taskModel)
        {
            // Ottieni i dettagli dei prodotti per ogni codice prodotto nella lista di codici prodotto del task
            var productViewModels = new List<ProductViewModel>();
            foreach (var productCode in taskModel.ProductCodeList)
            {
                // Ottieni il dettaglio del prodotto dal servizio di prodotto
                var productModel = productService.GetProductAsync(productCode).Result;
                // Mappa il ProductModel in ProductViewModel
                var productViewModel = _mapper.Map<ProductViewModel>(productModel);
                // Aggiungi il ProductViewModel alla lista di ProductViewModels
                productViewModels.Add(productViewModel);
            }

            // Mappa il TaskModel in TaskViewModel
            var orderViewModel = _mapper.Map<OrderViewModel>(taskModel);
            // Assegna la lista di ProductViewModels appena ottenuta a TaskViewModel.ProductList
            orderViewModel.ProductList = productViewModels;

            return orderViewModel;
        }
        private async Task<string> HttpOrderMicroserviceOperationAsync(string relativeExternalSubRoute, HttpMethod httpMethod, string serializedContent)
        {
            var relativeUrl = !string.IsNullOrEmpty(relativeExternalSubRoute) ? $"/{relativeExternalSubRoute}" : string.Empty;
            return await Utils.HttpUtils.ExternaMicroserviceHttpAsyncOperation($"{Constants.OrderMicroserviceRouteName}{relativeUrl}", httpMethod, serializedContent, _webServiceBaseUrl, _httpContextAccessor, _httpClient);
        }

        public async Task<List<OrderModel>> GetOrdersAsync()
        {
            var serializedResponse = await HttpOrderMicroserviceOperationAsync(null, HttpMethod.Get, null);
            return JsonConvert.DeserializeObject<List<OrderModel>>(serializedResponse);
        }

        public async Task<OrderModel> GetOrderAsync(string codOrder)
        {
            var serializedResponse = await HttpOrderMicroserviceOperationAsync(codOrder, HttpMethod.Get, null);
            return JsonConvert.DeserializeObject<OrderModel>(serializedResponse);
        }

        public async Task GenerateOrderAsync(OrderModel order)
        {
            var serializedOrder = JsonConvert.SerializeObject(order);
            await HttpOrderMicroserviceOperationAsync("generation", HttpMethod.Post, serializedOrder);
        }

        public async Task UpdateOrderAsync(string codOrder, OrderModel updatedOrder)
        {
            var serializedOrder = JsonConvert.SerializeObject(updatedOrder);
            await HttpOrderMicroserviceOperationAsync(codOrder, HttpMethod.Put, serializedOrder);
        }

        public async Task DeleteOrderAsync(string codOrder)
        {
            await HttpOrderMicroserviceOperationAsync(codOrder, HttpMethod.Delete, null);
        }
    }
}
