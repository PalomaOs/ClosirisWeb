using System.Security.Claims;
using closirissystem.Models;
using closirissystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace closirissystem;

public class SingUpController(UserClientService user) : Controller
{

    [AllowAnonymous]
    public IActionResult Index()
    {

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IndexAsync(User model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (model.ImageProfileFormFile != null)
                {
                    if ((model.ImageProfileFormFile.Length / 1024) > 100)
                    {
                        ModelState.AddModelError("ImageProfile", $"El archivo de {model.ImageProfileFormFile.Length / 1024} KB supera el tamaño máximo permitido.");

                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await model.ImageProfileFormFile.CopyToAsync(memoryStream);
                        model.ImageProfile = Convert.ToBase64String(memoryStream.ToArray());
                    }

                }


                await user.PostAsync(model);
                await user.PostAsync(model, "api/user");


                return RedirectToAction("Index", "Auth");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        return View(model);
    }


}