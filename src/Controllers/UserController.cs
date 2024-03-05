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
            // Logica per recuperare e mostrare l'elenco degli utenti
            var users = await _userService.GetUsersAsync(); // Aggiungi un metodo GetUsersAsync al tuo UserService

            // Passa la lista degli utenti alla vista
            return View(users.Select(x => _mapper.Map<UserViewModel>(x)).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Details(string codUser)
        {
            var user = await _userService.GetUserAsync(codUser); // Aggiungi un metodo GetUsersAsync al tuo UserService

            // Logica per recuperare e mostrare i dettagli di un utente specifico
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
                    HttpContext.Session.SetString("AuthToken", result.Token);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Login non valido");
            }

            // Se il modello non è valido o il login fallisce, rimani sulla pagina di login
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

                // Registrazione riuscita, puoi eseguire le operazioni necessarie e reindirizzare
                // l'utente a una pagina di conferma o a un'altra area.
                return RedirectToAction("Index", "Home");
            }

            // Se il modello non è valido, rimani sulla pagina di registrazione
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            // Rimuovi il token dalla sessione
            HttpContext.Session.Remove("AuthToken");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Esegui eventuali altre operazioni di logout necessarie

            // Reindirizza all'azione di login o ad un'altra pagina
            return RedirectToAction("Login", "User");
        }

    }
}
