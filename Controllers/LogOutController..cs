using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

public class LogoutController : Controller
{
    public IActionResult Index()
    {
        // Clear the existing user session or authentication token
        HttpContext.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }
}
