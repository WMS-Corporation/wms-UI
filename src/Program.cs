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
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

#region Mapper
var mapperConfig = new MapperConfiguration(config =>
{
    config.AddProfile<src.Mappings.MapperProfile>();
});

var mapper = mapperConfig.CreateMapper();

builder.Services.AddSingleton(mapper);
#endregion

var webServiceBaseUrl = @"http://localhost:3000";

// Configura il servizio con l'URL del servizio
builder.Services.AddScoped<UserService>(serviceProvider =>
    new UserService(
        serviceProvider.GetRequiredService<HttpClient>(),
        serviceProvider.GetRequiredService<IHttpContextAccessor>(),
        webServiceBaseUrl
    )
); 
builder.Services.AddScoped<ProductService>(serviceProvider =>
    new ProductService(
        serviceProvider.GetRequiredService<HttpClient>(),
        serviceProvider.GetRequiredService<IHttpContextAccessor>(),
        webServiceBaseUrl
    )
);
builder.Services.AddScoped<TaskService>(serviceProvider =>
    new TaskService(
        serviceProvider.GetRequiredService<HttpClient>(),
        serviceProvider.GetRequiredService<IHttpContextAccessor>(),
        webServiceBaseUrl,
        serviceProvider.GetRequiredService<IMapper>()
    )
);
builder.Services.AddScoped<OrderService>(serviceProvider =>
    new OrderService(
        serviceProvider.GetRequiredService<HttpClient>(),
        serviceProvider.GetRequiredService<IHttpContextAccessor>(),
        webServiceBaseUrl,
        serviceProvider.GetRequiredService<IMapper>()
    )
);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });


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

app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var authToken = context.Session.GetString(src.Constants.AuthToken);

    if (!string.IsNullOrEmpty(authToken))
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Username") }, "custom");

        identity.AddClaim(new Claim(src.Constants.AuthToken, authToken));

        var principal = new ClaimsPrincipal(identity);

        context.User = principal;

        var authenticationProperties = new AuthenticationProperties
        {
            IsPersistent = false
        };

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperties);
    }
    else
    {
        var contextRequestPath = context.Request.Path;
        if (contextRequestPath.StartsWithSegments("/User/Login") || contextRequestPath.StartsWithSegments("/User/Register"))
        {
            await next();
            return;
        }
        else
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
