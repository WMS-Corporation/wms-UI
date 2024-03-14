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
    public class UserController : Controller
    {
        private readonly UserService _userService;

        private readonly IMapper _mapper;
        public UserController(UserService userService, IMapper mapper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetUsersAsync();

            return View(users.Select(x => _mapper.Map<UserViewModel>(x)).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Details(string codUser)
        {
            var user = await _userService.GetUserAsync(codUser);

            return View(_mapper.Map<UserViewModel>(user));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginAsync(model);

                if (result != null)
                {
                    HttpContext.Session.SetString(Constants.AuthToken, result.Token);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Login non valido");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                await _userService.RegisterAsync(model);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove(Constants.AuthToken);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "User");
        }

    }
}
