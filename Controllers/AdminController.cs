using AspNetCore.Reporting;
using closirissystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace closirissystem;

public class AdminController : Controller
{
    private readonly UserClientService _user;
    private readonly IWebHostEnvironment? _webHostEnvironment;

    public AdminController(UserClientService user, IWebHostEnvironment webHostEnvironment)
    {
        _user = user;
        _webHostEnvironment = webHostEnvironment;
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Users()
    {
        var users = await _user.GetUsersAsync();
        return PartialView("_Users", users);
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Audit()
    {
        var audit = await _user.GetLogbooksAsync();
        return PartialView("_Audit", audit);
    }

    public async Task<IActionResult> UsersReport()
    {

        var path = Path.Combine(_webHostEnvironment.WebRootPath, "reports", "Users.rdlc");
        Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "prm", "Users Report" }
            };

        var localReport = new LocalReport(path);
        var users = await _user.GetUsersAsync();
        localReport.AddDataSource("DataSetUsers", users);

        var result = localReport.Execute(RenderType.Pdf, 1, parameters);

        string base64String = Convert.ToBase64String(result.MainStream);

        ViewBag.PdfBase64Users = base64String;

        return PartialView("_UsersReport");

    }

    public async Task<IActionResult> AuditReport()
    {
        var path = Path.Combine(_webHostEnvironment.WebRootPath, "reports", "Audit.rdlc");
        Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "prm", "Audit Report" }
            };

        var localReport = new LocalReport(path);
        var audit = await _user.GetLogbooksAsync();
        localReport.AddDataSource("DataSetAudit", audit);

        var result = localReport.Execute(RenderType.Pdf, 1, parameters);

        string base64String = Convert.ToBase64String(result.MainStream);

        ViewBag.PdfBase64Audit = base64String;

        return PartialView("_AuditReport");

    }
}
