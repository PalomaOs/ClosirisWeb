using closirissystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.Security.Claims;
using File = closirissystem.Models.File;

namespace closirissystem.Services;

public class UserClientService(HttpClient client)
{
    public async Task<bool> PostAsync(User user, string route)
    {
        var response = await client.PostAsJsonAsync($"api/user", user);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PostAsync(User user)
    {
        var response = await client.PostAsJsonAsync($"api/userAccount", user);
        return response.IsSuccessStatusCode;
    }

    public async Task<UserEdit?> GetAsync()
    {
        var userEdit = await client.GetFromJsonAsync<UserEdit>($"api/userInfo");
        return userEdit;
    }

    public async Task<bool> PutAsync(UserEdit user)
    {
        var response = await client.PutAsJsonAsync($"api/userAccount", user);
        return response.IsSuccessStatusCode;
    }

    public async Task<User> GetFoldersAsync(){
        User userClient = new User();
        var folders = await client.GetFromJsonAsync<List<string>>($"api/folders");
        
        folders.Add("Compartidos");

        userClient.Folders = folders;
        return userClient; 
    }

    public async Task<List<User>> GetUserShareFileAsync(int idFile){
        client.DefaultRequestHeaders.Remove("file_id");
        client.DefaultRequestHeaders.Add("file_id", idFile.ToString());

        
        var response = await client.GetAsync($"api/usersShare");
        var data = await response.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<List<User>>(data);;
        return responseData;
    }




}