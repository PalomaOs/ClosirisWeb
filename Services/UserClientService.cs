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

    public async Task<List<User>?> GetUsersAsync()
    {
        var users = await client.GetFromJsonAsync<List<User>>("api/users");
        return users;
    }

    public async Task<List<Logbook>?> GetLogbooksAsync()
    {
        var logbooks = await client.GetFromJsonAsync<List<Logbook>>("api/audit");
        return logbooks;
    }
}