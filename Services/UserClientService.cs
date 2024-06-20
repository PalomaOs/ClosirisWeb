using closirissystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

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

    public async Task<UserEdit?> GetAsync (){
        var userEdit = await client.GetFromJsonAsync<UserEdit>($"api/userInfo");
        return userEdit;
    }

    
}