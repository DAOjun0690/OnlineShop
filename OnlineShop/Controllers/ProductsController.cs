using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Core.Models;
using OnlineShop.Core.ViewModel;
using System.Linq.Expressions;
using OnlineShop.Services;

namespace OnlineShop.Controllers;

public class ProductsController : Controller
{
    private readonly OnlineShopContext _context;
    private readonly SiteSettingsService _siteSettingsService;

    public ProductsController(OnlineShopContext context, SiteSettingsService siteSettingsService)
    {
        _context = context;
        _siteSettingsService = siteSettingsService;
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
        Expression<Func<Product, bool>> lambda = null;
        if (cId != null)
        {
            lambda = p => (p.Status != ProductStatus.Draft && p.CategoryId.Equals(cId));
        }
        else
        {
            lambda = p => p.Status != ProductStatus.Draft;
        }

        // 獲取排序設定
        var productSortOrder = _siteSettingsService.GetProductSortOrder();

        // 根據排序設定進行排序
        IQueryable<Product> query = _context.Product
                                    .Include(p => p.Category)
                                    .Include(p => p.ProductStyles).AsNoTracking()
                                    .Where(lambda);

        // 根據排序設定進行排序
        IList<Product> products = productSortOrder switch
        {
            ProductSortOrder.IdAsc => await query.OrderBy(m => m.Id).ToListAsync(),
            ProductSortOrder.IdDesc => await query.OrderByDescending(m => m.Id).ToListAsync(),
            ProductSortOrder.PublishTimeAsc => await query.OrderBy(m => m.PublishTime).ToListAsync(),
            ProductSortOrder.PublishTimeDesc => await query.OrderByDescending(m => m.PublishTime).ToListAsync(),
            _ => await query.OrderByDescending(m => m.Id).ToListAsync()
        };

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
        viewModel.ProductImage = imageList.OrderBy(x => x.Sequence).ToList();

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
    public async Task<IActionResult> Details(int? id, string fromWhere)
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

        // 從哪個頁面跳轉而來
        viewModel.fromWhere = fromWhere;

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
