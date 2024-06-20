using System.Security.Claims;
using closirissystem.Services;

namespace closirissystem.Middlewares;

public class RefreshTokenDelegatingHandler(AuthClientService auth, IHttpContextAccessor httpContextAccessor) : DelegatingHandler {
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken){
        var response = await base.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        if(response.Headers.Contains("Set-Authorization")){
            string jwt = response.Headers.GetValues("Set-Authorization").FirstOrDefault()!;
            var claims = new List<Claim>{
                new (ClaimTypes.Name, httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)!),
                new (ClaimTypes.GivenName, httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.GivenName)!),
               // new (ClaimTypes.UserData, httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.UserData)!),
                new ("jwt", jwt),
                new (ClaimTypes.Role, httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role)!),
            };
            auth.LoginAsync(claims);
        }
        return response;
    }
}