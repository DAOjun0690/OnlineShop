using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.ViewModel;
using System.Diagnostics;

namespace OnlineShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            Console.WriteLine("進入首頁紀錄");
            return View();
        }

        /// <summary>
        /// 購物須知
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ShoppingNotice()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}