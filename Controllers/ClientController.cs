using System.Security.Claims;
using closirissystem.Models;
using closirissystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace closirissystem;

public class ClientController (UserClientService user) : Controller
{

    [AllowAnonymous]
    public async Task< IActionResult> Index()
    {
        await GetInfo();
        return View();
    }

    private async Task GetInfo (){
         UserEdit  userClient =  await user.GetAsync();
         
        Singleton.Instance.InfoUser = userClient;
    }

}