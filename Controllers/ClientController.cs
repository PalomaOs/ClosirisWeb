using System.Security.Claims;
using closirissystem.Models;
using closirissystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using File = closirissystem.Models.File;

namespace closirissystem;

public class ClientController(UserClientService user, FileClientService file) : Controller
{

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        await GetInfo();
        User userClient = new User();
        userClient = await user.GetFoldersAsync();

        return View(userClient);
    }

    private async Task GetInfo()
    {
        UserEdit userClient = await user.GetAsync();

        Singleton.Instance.InfoUser = userClient;
    }




    public async Task<IActionResult> EditAsync(UserEdit model)
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
                else
                {
                    model.ImageProfile = Singleton.Instance.InfoUser.ImageProfile;
                }



                await user.PutAsync(model);


                return RedirectToAction("Index", "Client");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        return RedirectToAction("Index", "Client");
    }


    public async Task<IActionResult> NewFolderAsync(File fileClient)
    {
        if (ModelState.IsValid)
        {
            try
            {
                fileClient.FileName = Path.GetFileName(fileClient.FileSelected.FileName);
                fileClient.CreationDate = DateTime.Now;

                var result = await file.PostAsync(fileClient);
                int id = (int)Singleton.Instance.IdFileUpload;
                var result2 = await file.PostAsync(id);
                return RedirectToAction("Index", "Client");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }
        return RedirectToAction("Index", "Client");
    }


}