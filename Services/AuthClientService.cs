using closirissystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace closirissystem.Services;

public class AuthClientService(HttpClient client, IHttpContextAccessor httpContextAccessor){
    public async Task<AuthUser> GetTokenAsync (string email, string password){
        Login user = new() {Email = email, Password = password};
        var response = await client.PostAsJsonAsync("api", user);
        var token = await response.Content.ReadFromJsonAsync<AuthUser>();
        return token!;
    }

    public async void LoginAsync(List<Claim> claims){
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties();
        await httpContextAccessor.HttpContext?.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties)!;
    }
}