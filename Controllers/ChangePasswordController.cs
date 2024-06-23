using closirissystem.Models;
using closirissystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace closirissystem;

public class ChangePasswordController(AuthClientService auth) : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        string email = TempData["Email"] as string;

        User model = new User { Email = email, Name = string.Empty, FreeStorage = 0m, Password = string.Empty, Plan = string.Empty };
        return View(model);
    }

    public async Task<IActionResult> ChangePasswordAsync(User model)
    {
        try
        {
            var isPasswordChanged = await auth.ChangePasswordAsync(model.Password);
            if (isPasswordChanged)
            {
                return RedirectToAction("Index", "Auth");
            }
        }
        catch (Exception ex)
        {
            return RedirectToAction("Index", "Auth");
        }

        return RedirectToAction("Index", "Auth");
    }

    public async Task<IActionResult> SendEmailAsync(User model)
    {
        var emailExists = await auth.ValidateEmailAsync(model.Email);
        if (emailExists)
        {
            Email email = new Email();
            string token = email.GenerateToken();
            Singleton.Instance.Token = token;
            Singleton.Instance.Email = model.Email;
            string text = email.Format(token);
            email.SendEmail("Recuperar cuenta", text, model.Email);

            TempData["Email"] = model.Email;
        }
        return RedirectToAction("Index", "ChangePassword");
    }
}