using closirissystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using System.Security.Claims;
using File = closirissystem.Models.File;

namespace closirissystem.Services;

public class FileClientService(HttpClient client)
{
    public async Task<bool> PostAsync(File file)
    {
        byte[] fileBytes;

        using (var fileStream = new FileStream(file.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }
        }

        Console.WriteLine($"Archivo a enviar: {file.FileName}");
        Console.WriteLine($"Tamaño del archivo: {fileBytes.Length} bytes");
        Console.WriteLine($"Carpeta destino: {file.FolderName}");

        using (var content = new MultipartFormDataContent())
        {
            var fileContent = new ByteArrayContent(fileBytes);
            //fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            content.Add(fileContent, "file", file.FileName);
            client.DefaultRequestHeaders.Remove("folder_name");
            content.Headers.Add("folder_name", file.FolderName);

            Console.WriteLine($"Se envia: {content}");

            var response = await client.PostAsync($"api/file", content);
            return response.IsSuccessStatusCode;
        }

    }
}
