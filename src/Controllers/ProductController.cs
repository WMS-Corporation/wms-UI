using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using src.Models;
using src.Services;
using src.ViewModel;

namespace src.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(ProductService productService, IMapper mapper)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProductsAsync();
            var productViewModels = products.Select(p => _mapper.Map<ProductViewModel>(p)).ToList();
            return View("Index", productViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string codProduct)
        {
            var product = await _productService.GetProductAsync(codProduct);
            return View("Details", _mapper.Map<ProductViewModel>(product));
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View("Add", new ProductViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var productModel = _mapper.Map<ProductModel>(productViewModel);
                await _productService.AddProductAsync(productModel);
                return RedirectToAction("Index");
            }

            return View("Add", productViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string codProduct)
        {
            var product = await _productService.GetProductAsync(codProduct);
            return View("Edit", _mapper.Map<ProductViewModel>(product));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string codProduct, ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var productModel = _mapper.Map<ProductModel>(productViewModel);
                await _productService.UpdateProductAsync(codProduct, productModel);
                return RedirectToAction("Index");
            }

            return View("Edit", productViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string codProduct)
        {
            await _productService.DeleteProductAsync(codProduct);
            return RedirectToAction("Index");
        }
    }
}
