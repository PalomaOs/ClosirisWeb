using System.Security.Claims;
using closirissystem.Models;
using closirissystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace closirissystem;

public class AuthController (AuthClientService auth) : Controller {

    [AllowAnonymous]
    public IActionResult Index(){
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IndexAsync (Login model){
        if (ModelState.IsValid){
            try {
                var token = await auth.GetTokenAsync(model.Email, model.Password);
                var claims = new List<Claim>{
                    new (ClaimTypes.Name, token.Email),
                    new (ClaimTypes.GivenName, token.Name),
                    new ("jwt", token.Jwt),
                    new (ClaimTypes.Role, token.Role),

                };

                auth.LoginAsync(claims);

                return RedirectToAction("Index", "Client");
            }catch(Exception ex){
                Console.WriteLine($"Exception: {ex.Message}");
                ModelState.AddModelError("Email", "Credenciales no válidas, inténtelo nuevamente.");
            }
        }
        return View(model);
    }

    [Authorize(Roles ="Administrador, Premiun, Básico")]
    public async Task<IActionResult> SalirAsync(){
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Auth");
    }
}