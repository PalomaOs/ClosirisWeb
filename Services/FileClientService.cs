using closirissystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Security.Claims;
using File = closirissystem.Models.File;

namespace closirissystem.Services;

public class FileClientService(HttpClient client)
{
    public async Task<bool> PostAsync(File file)
    {
        using (var stream = file.FileSelected.OpenReadStream())
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                Console.WriteLine($"Archivo a enviar: {file.FileName}");
                Console.WriteLine($"Tamaño del archivo: {fileBytes.Length} bytes");
                Console.WriteLine($"Carpeta destino: {file.FolderName}");

                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(fileBytes);
                    content.Add(fileContent, "file", file.FileName);
                    client.DefaultRequestHeaders.Remove("folder_name");
                    content.Headers.Add("folder_name", file.FolderName);

                    Console.WriteLine($"Se envia: {content}");

                    var response = await client.PostAsync($"api/file", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<dynamic>(responseBody);
                        var fileId = responseData.id;
                        Singleton.Instance.IdFileUpload = fileId;
                        Console.WriteLine($"ID del archivo creado: {fileId}");

                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Error al crear archivo: {response.StatusCode}");
                        return false;
                    }
                }
            }
        }
    }

    public async Task<bool> PostAsync(int idFile)
    {
        client.DefaultRequestHeaders.Remove("file_id");
        client.DefaultRequestHeaders.Add("file_id", idFile.ToString());


        var response = await client.PostAsync($"api/fileowner", null);
        return response.IsSuccessStatusCode;
    }


}
