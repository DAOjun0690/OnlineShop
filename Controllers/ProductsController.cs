using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

public class ProductsController : Controller
{
    private readonly OnlineShopContext _context;

    public ProductsController(OnlineShopContext context)
    {
        _context = context;
    }

    // GET: Products
    public async Task<IActionResult> Index(int? cId)
    {
        List<DetailViewModel> dvm = new List<DetailViewModel>();
        List<Product> products = new List<Product>();
        if (cId != null)
        {
            var result = await _context.Category.SingleAsync(x => x.Id.Equals(cId));
            products = await _context.Entry(result).Collection(x => x.Products).Query().ToListAsync();
        }
        else
        {
            products = await _context.Product.Include(p => p.Category).ToListAsync();
        }

        foreach (var product in products)
        {
            DetailViewModel item = new DetailViewModel
            {
                product = product,
                //imgsrc = ViewImage(product.Image)
            };
            dvm.Add(item);
        }
        ViewBag.count = dvm.Count;

        return View(dvm);
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        DetailViewModel dvm = new DetailViewModel();

        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Product.Include(p => p.Category)
                                            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        else
        {
            dvm.product = product;
            //dvm.imgsrc = ViewImage(product.Image);
        }

        return View(dvm);
    }


    private bool ProductExists(int id)
    {
        return _context.Product.Any(e => e.Id == id);
    }

    private string ViewImage(byte[] arrayImage)
    {
        string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
        return "data:image/png;base64," + base64String;
    }
}
