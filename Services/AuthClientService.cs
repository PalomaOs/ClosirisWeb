using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using closirissystem.Models;

namespace closirissystem.Services
{
    public class AuthClientService
    {
        private readonly HttpClient client;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthClientService(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthUser> GetTokenAsync(string email, string password)
        {
            Login user = new() { Email = email, Password = password };
            var response = await client.PostAsJsonAsync("api", user);
            var token = await response.Content.ReadFromJsonAsync<AuthUser>();
            return token!;
        }

        public async Task LoginAsync(List<Claim> claims)
        {
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();
            await httpContextAccessor.HttpContext?.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties)!;
        }

        public async Task<bool> ValidateEmailAsync(string email)
        {
            var response = await client.GetAsync($"api/emailDuplicity/{email}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ChangePasswordAsync(string newPassword)
        {
            var data = new { email = Singleton.Instance.Email, password = newPassword };
            var response = await client.PatchAsJsonAsync("api/password", data);
            return response.IsSuccessStatusCode;
        }
    }
}
