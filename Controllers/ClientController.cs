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

        int totalStorageMB = 0;
        if (Singleton.Instance.InfoUser.Plan == "Premium")
        {
            totalStorageMB = 100;
        }
        else if (Singleton.Instance.InfoUser.Plan == "Básico")
        {
            totalStorageMB = 50;
        }

        double freeStorageMB = (double)Singleton.Instance.InfoUser.FreeStorage / 1048576.0;
        double usedStoragePercentage = (freeStorageMB / totalStorageMB) * 100;

        ViewBag.UsedStoragePercentage = usedStoragePercentage;
        ViewBag.TotalStorageMB = totalStorageMB;
        ViewBag.FreeStorageMB = freeStorageMB;
        ViewBag.PlanType = Singleton.Instance.InfoUser.Plan;

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
            if (fileClient == null)
            {
                Console.WriteLine("Error: fileClient es null.");
                return RedirectToAction("Index", "Client");
            }

            if (fileClient.FileSelected == null)
            {
                Console.WriteLine("Error: fileClient.FileSelected es null.");
                return RedirectToAction("Index", "Client");
            }

            if (fileClient.FolderName == null)
            {
                Console.WriteLine("Error: fileClient.FolderName es null.");
                return RedirectToAction("Index", "Client");
            }

            try
            {

                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var fileName = Path.GetFileName(fileClient.FileSelected.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileClient.FileSelected.CopyToAsync(stream);
                }

                fileClient.FileName = fileName;
                fileClient.FilePath = filePath;
                fileClient.CreationDate = DateTime.Now;

                // Aquí puedes agregar la lógica para guardar la información del archivo en tu base de datos

                var result = await file.PostAsync(fileClient);


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

    public async Task<IActionResult> UpdateUserPlanAsync(User userModel)
    {
        try
        {
            var freeStorage = Singleton.Instance.InfoUser.FreeStorage;
            var differenceStorage = 52428800 - freeStorage;
            userModel.FreeStorage = (decimal)(104857600 - differenceStorage);

            await user.UpdateUserPlanAsync(userModel);

            return RedirectToAction("Index", "Auth");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }

        return RedirectToAction("Index", "Client");
    }
}