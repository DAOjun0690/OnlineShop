using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Core.Models;
using OnlineShop.Core.ViewModel;

namespace OnlineShop.Controllers;

public class ProductsController : Controller
{
    private readonly OnlineShopContext _context;

    public ProductsController(OnlineShopContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cId"></param>
    /// <returns></returns>
    [HttpGet]
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
            products = await _context.Product
                                .Include(p => p.Category)
                                .Include(p => p.ProductStyles).AsNoTracking()
                                .Where(p => p.Status != ProductStatus.Draft).ToListAsync();
        }

        foreach (var product in products)
        {
            var image = _context.ProductImage
                .Where(x => x.ProductId == product.Id && x.Type == ProductImageType.Detail)
                .FirstOrDefault();

            DetailViewModel item = new DetailViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Image = image,
                MinPrice = product.ProductStyles.Min(ps => ps.Price),
                MaxPrice = product.ProductStyles.Max(ps => ps.Price),
                Stock = product.ProductStyles.Sum(ps => ps.Stock),
                Status = product.Status
            };
            dvm.Add(item);
        }

        return View(dvm);
    }

    /// <summary>
    /// 簡易 購買頁面
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> SubDetails(int? id)
    {
        if (id == null || _context.Product == null)
        {
            return NotFound();
        }

        var product = _context.Product.Include(p => p.Category)
                                             .Include(p => p.ProductStyles).AsNoTracking()
                                             .FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        MapperConfiguration config = new MapperConfiguration(cfg =>
            cfg.CreateMap<Product, ProductDetailViewModel>());

        //Using automapper
        Mapper mapper = new Mapper(config);
        ProductDetailViewModel viewModel = mapper.Map<ProductDetailViewModel>(product);
        viewModel.ProductId = product.Id;
        viewModel.CategoryName = product.Category.Name;

        // 再從db撈 當前 productId的 detail圖片
        var imageList =
            _context.ProductImage.Where(x => x.ProductId == product.Id && x.Type == ProductImageType.Detail);
        viewModel.ProductImage = imageList.ToList();

        // 商品狀態
        viewModel.Status = product.Status;

        // 商品款式
        viewModel.ProductStylesList = product.ProductStyles.ToList();

        return PartialView(viewModel);
    }

    /// <summary>
    /// 商品 詳細資料 購買頁面
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Product == null)
        {
            return NotFound();
        }

        var product = _context.Product.Include(p => p.Category)
                                             .Include(p => p.ProductStyles).AsNoTracking()
                                             .FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        MapperConfiguration config = new MapperConfiguration(cfg =>
            cfg.CreateMap<Product, ProductDetailViewModel>());

        //Using automapper
        Mapper mapper = new Mapper(config);
        ProductDetailViewModel viewModel = mapper.Map<ProductDetailViewModel>(product);
        viewModel.ProductId = product.Id;
        viewModel.CategoryName = product.Category.Name;

        // 再從db撈 當前 productId的 detail圖片
        var imageList =
            _context.ProductImage.Where(x => x.ProductId == product.Id && x.Type == ProductImageType.Detail);
        viewModel.ProductImage = imageList.ToList();

        // 商品狀態
        viewModel.Status = product.Status;

        // 商品款式
        viewModel.ProductStylesList = product.ProductStyles.ToList();

        return View(viewModel);
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
