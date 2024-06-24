using System.Globalization;
using System.Text;
using closirissystem.Models;
using closirissystem.Services;
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

        double roundedFreeStorageMB = Math.Round(freeStorageMB, 2);

        ViewBag.UsedStoragePercentage = usedStoragePercentage;
        ViewBag.TotalStorageMB = totalStorageMB;
        ViewBag.FreeStorageMB = roundedFreeStorageMB;
        ViewBag.PlanType = Singleton.Instance.InfoUser.Plan;

        return View(userClient);
    }
    [HttpPost]
    public async Task<IActionResult> ValidateEmailDuplicity(string Email, int fileId)
    {
        bool isDuplicate = await user.ValidateEmailDuplicityAsync(Email);
        File fileClient = Singleton.Instance.filesFolder.FirstOrDefault(f => f.Id == fileId);
        if (isDuplicate)
        {
            User userModel = await user.GetUserInfoByEmailAsync(Email);
            await file.PostAsync(userModel.Id, fileClient.Id);
            return RedirectToAction("Index", "Client");
        }

        return RedirectToAction("Index", "Client");
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
                fileClient.Size = fileClient.FileSelected.Length.ToString();
                fileClient.CreationDate = DateTime.Now;
                var result = await file.PostAsync(fileClient);
                int id = (int)Singleton.Instance.IdFileUpload;

                decimal? freeStorage = Singleton.Instance.InfoUser.FreeStorage;
                decimal storageToUpdate = 0;

                if (freeStorage.HasValue)
                {
                    if (decimal.TryParse(fileClient.Size, out decimal fileSize))
                    {
                        storageToUpdate = freeStorage.Value - fileSize;
                    }
                }

                await UpdateFreeStorageAsync(storageToUpdate);

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

    [HttpGet]
    public async Task<IActionResult> ListFiles([FromQuery] string folderName)
    {
        File filesUser = new File();

        List<File>? allFiles = [];
        allFiles = await file.GetAsync(folderName);

        foreach (var file in allFiles)
        {
            string extension = System.IO.Path.GetExtension(file.FileName).ToLower();
            file.FileImage = GetIcon(extension);
            file.FileExtension = extension;
            file.FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            decimal.TryParse(file.Size, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var sizeInBytes);
            file.FileSize = Math.Round((double)sizeInBytes / 1024, 2);
        }
        Singleton.Instance.filesFolder = allFiles;
        filesUser.FolderName = folderName;
        filesUser.Files = allFiles;

        return PartialView("_ListFiles", filesUser);
    }

    [HttpGet]
    public async Task<IActionResult> FileInfo([FromQuery] int fileId, [FromQuery] string fileName, [FromQuery] string fileExtension, [FromQuery] double fileSize, [FromQuery] DateTime creationDate, [FromQuery] string folderName)

    {

        SetInfoShare(fileId);
        SetCreationDate(fileId);

        string data = await file.GetDataFileAsync(fileId);

        File fileInfo = new File
        {
            Id = fileId,
            PreviewFile = data,
            FileName = fileName,
            FileImage = GetIcon(fileExtension),
            FileExtension = fileExtension,
            FileSize = fileSize,
            FolderName = folderName
        };
        return PartialView("_FileInfo", fileInfo);
    }

    private async void SetInfoShare(int fileId)
    {
        List<User> users = await user.GetUserShareFileAsync(fileId);
        string usersShare;
        string share = "";
        if (users != null && users.Count > 0)
        {
            foreach (var user in users)
            {
                share = share + user.Name + ", ";
            }
            usersShare = "Creado por ti. Compartido con " + share + ".";
        }
        else
        {
            usersShare = "Creado por ti";
        }
        ViewBag.Share = usersShare;
    }

    private void SetCreationDate(int fileId)
    {
        File fileDate = Singleton.Instance.filesFolder.FirstOrDefault(f => f.Id == fileId);
        DateTime dateTime;
        if (fileDate != null)
        {
            dateTime = (DateTime)fileDate.CreationDate;
            string date = dateTime.ToString("dd/MM/yyyy");
            ViewBag.CreationDate = date;
        }
    }


    private string GetIcon(string extension)
    {
        string icon;
        switch (extension)
        {
            case ".jpeg":
            case ".png":
            case ".jpg":
            case ".gif":
                icon = "https://img.icons8.com/?size=100&id=bjHuxcHTNosO&format=png&color=000000";
                break;
            case ".pdf":
                icon = "https://img.icons8.com/?size=100&id=mcyAsTDJNTI9&format=png&color=000000";
                break;
            case ".docx":
                icon = "https://img.icons8.com/?size=100&id=117563&format=png&color=000000";
                break;
            case ".txt":
                icon = "https://img.icons8.com/?size=100&id=PewvBGCwMClZ&format=png&color=000000";
                break;
            case ".csv":
                icon = "https://img.icons8.com/?size=100&id=9LTQ9vunHbhK&format=png&color=000000";
                break;
            case ".mp3":
                icon = "https://img.icons8.com/?size=100&id=WYM9OeqhxV0K&format=png&color=000000";
                break;
            case ".mp4":
                icon = "https://img.icons8.com/?size=100&id=Fh3zqn8XpE89&format=png&color=000000";
                break;
            case ".xlsx":
                icon = "https://img.icons8.com/?size=100&id=13425&format=png&color=000000";
                break;
            default:
                icon = "https://img.icons8.com/?size=100&id=XWoSyGbnshH2&format=png&color=000000";
                break;
        }
        return icon;
    }

    public async Task<IActionResult> DownloadFileAsync(int fileId)
    {
        File fileClient = Singleton.Instance.filesFolder.FirstOrDefault(f => f.Id == fileId);

        if (fileClient == null)
        {
            return NotFound();
        }

        var data = await file.GetDataFileAsync(fileClient.Id);

        if (data == null)
        {
            return NotFound();
        }
        byte[] fileBytes = Convert.FromBase64String(data);

        string fileNameWithExtension = fileClient.FileName + fileClient.FileExtension;

        return File(fileBytes, "application/octet-stream", fileNameWithExtension);
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

    public async Task<bool> UpdateFreeStorageAsync(decimal storage)
    {
        return await user.UpdateFreeStorageAsync(storage);
    }

    public async Task<string> GetDataFileAsync(int idFile)
    {
        return await file.GetDataFileAsync(idFile);
    }

    public async Task<IActionResult> DeleteFileAsync(int fileId)
    {
        File fileClient = Singleton.Instance.filesFolder.FirstOrDefault(f => f.Id == fileId);
        decimal.TryParse(fileClient.Size, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var sizeInBytes);
        decimal storageToUpdate = 0;

        if (Singleton.Instance.InfoUser.FreeStorage.HasValue)
        {
            storageToUpdate = Singleton.Instance.InfoUser.FreeStorage.Value + sizeInBytes;
        }

        await UpdateFreeStorageAsync(storageToUpdate);

        await DeleteFileFromServerAsync(fileId);
        await DeleteFileRegistrationAsync(fileId);

        return RedirectToAction("Index", "Client");
    }

    public async Task<bool> DeleteFileRegistrationAsync(int idFile)
    {
        return await file.DeleteFileRegistrationAsync(idFile);
    }

    public async Task<bool> DeleteFileFromServerAsync(int idFile)
    {
        return await file.DeleteFileFromServerAsync(idFile);
    }

    public async Task<bool> DeleteFileShared(int idFile)
    {
        return await file.DeleteFileShared(idFile);
    }
}