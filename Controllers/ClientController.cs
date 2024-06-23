using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
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

    private async void SetInfoShare(int fileId){
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

    private void SetCreationDate (int fileId){
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


}