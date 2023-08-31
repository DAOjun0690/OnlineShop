using Microsoft.AspNetCore.Mvc;

namespace OnlineShop.Controllers;

public class HelloWorldController : Controller
{
    public IActionResult Index()
    {
        ViewData["mystring1"] = "I am ViewData !";
        ViewBag.mystring2 = "I am ViewBag !";
        return View();
    }

    public string Welcome(string Name)
    {
        return "This is the Welcome Action : Hello, " + Name + " !!";
    }
}
