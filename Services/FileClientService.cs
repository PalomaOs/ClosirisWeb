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


                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(fileBytes);
                    content.Add(fileContent, "file", file.FileName);
                    client.DefaultRequestHeaders.Remove("folder_name");
                    content.Headers.Add("folder_name", file.FolderName);


                    var response = await client.PostAsync($"api/file", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<dynamic>(responseBody);
                        var fileId = responseData.id;
                        Singleton.Instance.IdFileUpload = fileId;

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

    public async Task<List<File>>? GetAsync(string folderName)
    {
        client.DefaultRequestHeaders.Remove("folder_name");
        client.DefaultRequestHeaders.Add("folder_name", folderName);
        var allFiles = await client.GetFromJsonAsync<List<File>>($"api/fileInfo");


        return allFiles;
    }

    public async Task<string> GetDataFileAsync(int idFile)
    {
        client.DefaultRequestHeaders.Remove("file_id");
        client.DefaultRequestHeaders.Add("file_id", idFile.ToString());

        var response = await client.GetAsync($"api/file");
        var data = await response.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<dynamic>(data);
        var fileBase64 = responseData.fileBase64;
        return fileBase64;
    }

    public async Task<string> GetDataFileAsync(int idFile)
    {
        client.DefaultRequestHeaders.Remove("file_id");
        client.DefaultRequestHeaders.Add("file_id", idFile.ToString());

        var response = await client.GetAsync($"api/file");
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<bool> DeleteFileRegistrationAsync(int idFile)
    {
        client.DefaultRequestHeaders.Remove("file_id");
        client.DefaultRequestHeaders.Add("file_id", idFile.ToString());

        var response = await client.DeleteAsync($"api/file");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteFileFromServerAsync(int idFile)
    {
        client.DefaultRequestHeaders.Remove("file_id");
        client.DefaultRequestHeaders.Add("file_id", idFile.ToString());

        var response = await client.DeleteAsync($"api/serverFile");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteFileShared(int idFile)
    {
        client.DefaultRequestHeaders.Remove("file_id");
        client.DefaultRequestHeaders.Add("file_id", idFile.ToString());

        var response = await client.DeleteAsync($"api/fileShared");
        return response.IsSuccessStatusCode;
    }
}
