using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using src.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Configura il servizio di sessione
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".YourApp.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Imposta il timeout della sessione
});

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

var webServiceBaseUrl = @"http://localhost:3000"; // Sostituisci con il tuo effettivo URL del servizio

// Configura il servizio con l'URL del servizio
builder.Services.AddScoped<UserService>(serviceProvider =>
    new UserService(
        serviceProvider.GetRequiredService<HttpClient>(),
        serviceProvider.GetRequiredService<IHttpContextAccessor>(),
        webServiceBaseUrl
    )
);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        //options.Cookie.Name = "AuthToken";
        options.LoginPath = "/User/Login"; // Imposta il percorso di accesso per il reindirizzamento
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Imposta la durata del cookie
    });

var mapperConfig = new MapperConfiguration(config =>
{
    config.AddProfile<src.Mappings.UserMappingProfile>();
});

var mapper = mapperConfig.CreateMapper();

builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        // Verifica se l'errore è relativo all'autenticazione
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is UnauthorizedAccessException)
        {
            // Reindirizza alla pagina di logout
            context.Response.Redirect("/User/Logout");
            return;
        }
        else
        {
            context.Response.Redirect("/Home/Error");
            return;
        }
    });
});

// ... altri middleware ...
app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Aggiungi il middleware di sessione
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

////gestione token per autenticazione asp.net
//app.UseWhen(context => !context.Request.Path.StartsWithSegments("/User/Login"), appBuilder =>
//{
//    appBuilder.Use(async (context, next) =>
//    {
//        var authToken = context.Session.GetString("AuthToken");

//        if (!string.IsNullOrEmpty(authToken))
//        {
//            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Username") }, "custom");

//            // Associa il token all'identità
//            identity.AddClaim(new Claim("AuthToken", authToken));

//            var principal = new ClaimsPrincipal(identity);

//            // Imposta l'utente come autenticato
//            context.User = principal;

//            var authenticationProperties = new AuthenticationProperties
//            {
//                IsPersistent = false
//            };

//            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperties);
//        }
//        else
//        {
//            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//            // Non autenticato, reindirizza alla rotta di login
//            context.Response.Redirect("/User/Login");
//            return;
//        }

//        await next();
//    });
//});

app.Use(async (context, next) =>
{
    var authToken = context.Session.GetString("AuthToken");

    if (!string.IsNullOrEmpty(authToken))
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Username") }, "custom");

        // Associa il token all'identità
        identity.AddClaim(new Claim("AuthToken", authToken));

        var principal = new ClaimsPrincipal(identity);

        // Imposta l'utente come autenticato
        context.User = principal;

        var authenticationProperties = new AuthenticationProperties
        {
            IsPersistent = false
        };

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperties);
    }
    else
    {
        if (context.Request.Path.StartsWithSegments("/User/Login"))
        {
            // Se è per la pagina di login, passa alla catena di middleware successiva
            await next();
            return;
        }
        else
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Non autenticato, reindirizza alla rotta di login
            context.Response.Redirect("/User/Login");
            return;
        }
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "login",
    pattern: "User/Login",
    defaults: new { controller = "User", action = "Login" });

app.Run();
