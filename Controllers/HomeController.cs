using Microsoft.AspNetCore.Mvc;

namespace closirissystem;

public class HomeController : Controller {
    public IActionResult Index() {
        return View();
    }

    public IActionResult Error ([FromServices] IHostEnvironment hostEnvironment){
        return View();
    }

    public IActionResult AccessDenied() {
        return View();
    }

}