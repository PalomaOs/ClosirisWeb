using System.Security.Claims;
using closirissystem.Models;
using closirissystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace closirissystem;

public class AdminController(UserClientService user) : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Users()
    {
        var users = await user.GetUsersAsync();
        return PartialView("_Users", users);
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Audit()
    {
        var audit = await user.GetLogbooksAsync();
        return PartialView("_Audit", audit);
    }
}