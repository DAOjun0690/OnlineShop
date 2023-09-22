using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;

namespace OnlineShop.ViewComponents;

public class Carousel : ViewComponent
{
    private readonly OnlineShopContext _context;

    public Carousel(OnlineShopContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}
