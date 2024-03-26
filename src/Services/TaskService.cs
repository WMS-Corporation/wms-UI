using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
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
    public class TaskService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _webServiceBaseUrl;
        private readonly IMapper _mapper;


        public TaskService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, string webServiceBaseUrl, IMapper mapper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _webServiceBaseUrl = webServiceBaseUrl ?? throw new ArgumentNullException(nameof(webServiceBaseUrl));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<TaskViewModel> MapTaskModelToViewModel(ProductService productService, TaskModel taskModel)
        {
            // Ottieni i dettagli dei prodotti per ogni codice prodotto nella lista di codici prodotto del task
            var productViewModels = new List<ProductViewModel>();
            foreach (var productCode in taskModel.ProductCodeList)
            {
                // Ottieni il dettaglio del prodotto dal servizio di prodotto
                var productModel = await productService.GetProductAsync(productCode);
                // Mappa il ProductModel in ProductViewModel
                var productViewModel = _mapper.Map<ProductViewModel>(productModel);
                // Aggiungi il ProductViewModel alla lista di ProductViewModels
                productViewModels.Add(productViewModel);
            }

            // Mappa il TaskModel in TaskViewModel
            var taskViewModel = _mapper.Map<TaskViewModel>(taskModel);
            // Assegna la lista di ProductViewModels appena ottenuta a TaskViewModel.ProductList
            taskViewModel.ProductList = productViewModels;

            return taskViewModel;
        }

        private async Task<string> HttpTaskMicroserviceOperationAsync(string relativeExternalSubRoute, HttpMethod httpMethod, string serializedContent)
        {
            var relativeUrl = !string.IsNullOrEmpty(relativeExternalSubRoute) ? $"/{relativeExternalSubRoute}" : string.Empty;
            return await Utils.HttpUtils.ExternaMicroserviceHttpAsyncOperation($"{Constants.TaskMicroserviceRouteName}{relativeUrl}", httpMethod, serializedContent, _webServiceBaseUrl, _httpContextAccessor, _httpClient);
        }

        public async Task AddTaskAsync(TaskModel task)
        {
            var serializedTask = JsonConvert.SerializeObject(task);
            await HttpTaskMicroserviceOperationAsync(null, HttpMethod.Post, serializedTask);
        }

        public async Task UpdateTaskAsync(TaskModel updatedTask)
        {
            var serializedUpdatedTask = JsonConvert.SerializeObject(updatedTask);
            await HttpTaskMicroserviceOperationAsync(updatedTask.CodTask, HttpMethod.Put, serializedUpdatedTask);
        }

        public async Task DeleteTaskAsync(string taskId)
        {
            await HttpTaskMicroserviceOperationAsync(taskId, HttpMethod.Delete, null);
        }

        public async Task<TaskModel> GetTaskAsync(string taskId)
        {
            var serializedResponse = await HttpTaskMicroserviceOperationAsync(taskId, HttpMethod.Get, null);
            return JsonConvert.DeserializeObject<TaskModel>(serializedResponse);
        }

        public async Task<List<TaskModel>> GetTasksAsync()
        {
            var serializedResponse = await HttpTaskMicroserviceOperationAsync("all", HttpMethod.Get, null);
            return JsonConvert.DeserializeObject<List<TaskModel>>(serializedResponse);
        }
    }
}
