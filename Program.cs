using closirissystem.Middlewares;
using closirissystem. Services;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);
// Agregamos los servicios
builder.Services.AddControllersWithViews();
// Soporte para consultar el API
var UrlWebAPI = builder.Configuration["UrlWebAPI"];
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<SendBearerDelegatingHandler>();
builder.Services.AddTransient<RefreshTokenDelegatingHandler>();
builder.Services.AddHttpClient<AuthClientService>(httpClient => { httpClient.BaseAddress = new Uri(UrlWebAPI!); });
builder.Services.AddHttpClient<UserClientService>(httpClient => { httpClient.BaseAddress = new Uri(UrlWebAPI!); })
.AddHttpMessageHandler<SendBearerDelegatingHandler>()
.AddHttpMessageHandler<RefreshTokenDelegatingHandler>();
builder.Services.AddHttpClient<FileClientService>(httpClient => { httpClient.BaseAddress = new Uri(UrlWebAPI!); })
.AddHttpMessageHandler<SendBearerDelegatingHandler>()
.AddHttpMessageHandler<RefreshTokenDelegatingHandler>() ;

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>{
    options.Cookie.Name = ".closiris";
options.AccessDeniedPath = "/Home/AccessDenied";
options.LoginPath = "/Auth";
options. SlidingExpiration = true;
options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
}) ;

var app = builder.Build();

app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();
app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app. Run ();