using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Core.Dto;
using OnlineShop.Data;
using OnlineShop.Core.Models;
using OnlineShop.Core.ViewModel;
using OnlineShop.Services;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin")]
public class ProductManagementController : Controller
{
    private readonly OnlineShopContext _context;
    private readonly List<SelectListItem> _productStatusEnumList;
    private readonly SiteSettingsService _siteSettingsService;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="context"></param>
    /// <param name="siteSettingsService"></param>
    public ProductManagementController(OnlineShopContext context, SiteSettingsService siteSettingsService)
    {
        _context = context;
        _siteSettingsService = siteSettingsService;
        _productStatusEnumList = Enum.GetValues(typeof(ProductStatus))
                             .Cast<ProductStatus>()
                             .Select(e => new SelectListItem
                             {
                                 Value = ((int)e).ToString(),
                                 Text = e.GetDescription()
                             })
                             .ToList();
    }

    /// <summary>
    /// 取得 管理者 商品列表 資料
    /// </summary>
    /// <param name="searchString">商品名稱 like 查詢字串</param>
    /// <param name="currentFilter">舊有的查詢條件</param>
    /// <param name="pageNumber">第幾頁</param>
    /// <param name="status">商品狀態</param>
    /// <param name="sortOrder">排序方式</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Index(string searchString,
                                           string currentFilter,
                                           int? pageNumber,
                                           ProductStatus status,
                                           string sortOrder = "")
    {
        // 儲存當前搜尋狀態
        ViewData["CurrentFilter"] = searchString ?? currentFilter;
        // 商品狀態列表
        ViewData["StatusList"] = new SelectList(_productStatusEnumList, "Value", "Text", (int)status);
        ViewData["StatusName"] = status != 0 ? status.GetDescription() : "全部";

        // 排序相關
        ViewData["CurrentSort"] = sortOrder;
        ViewData["IdSortParam"] = sortOrder == "id_desc" ? "id" : "id_desc";
        ViewData["PublishTimeSortParam"] = sortOrder == "publish_desc" ? "publish" : "publish_desc";

        var result = _context.Product.AsQueryable();

        // 商品名稱 模糊查詢
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            result = result.Where(s => s.Name.Contains(searchString));
        }
        // 篩選 符合的商品狀態
        if (status != 0)
        {
            result = result.Where(p => p.Status == status);
        }

        // 如果有指定排序方式，則保存排序設定
        if (!string.IsNullOrEmpty(sortOrder))
        {
            // 將排序方式轉換為 ProductSortOrder 枚舉
            ProductSortOrder productSortOrder = sortOrder switch
            {
                "id" => ProductSortOrder.IdAsc,
                "id_desc" => ProductSortOrder.IdDesc,
                "publish" => ProductSortOrder.PublishTimeAsc,
                "publish_desc" => ProductSortOrder.PublishTimeDesc,
                _ => ProductSortOrder.IdDesc
            };

            // 保存排序設定
            _siteSettingsService.SetProductSortOrder(productSortOrder);
        }

        // 獲取排序設定
        var productSortOrderSetting = _siteSettingsService.GetProductSortOrder();

        // 根據排序設定進行排序
        var orderedResult = productSortOrderSetting switch
        {
            ProductSortOrder.IdAsc => result.OrderBy(m => m.Id),
            ProductSortOrder.IdDesc => result.OrderByDescending(m => m.Id),
            ProductSortOrder.PublishTimeAsc => result.OrderBy(m => m.PublishTime),
            ProductSortOrder.PublishTimeDesc => result.OrderByDescending(m => m.PublishTime),
            _ => result.OrderByDescending(m => m.Id)
        };

        // 列表一頁顯示筆數
        int pageSize = 10;
        return View(await PaginatedList<Product>.CreateAsync(
            orderedResult.Include(p => p.Category)
                         .Include(p => p.ProductStyles)
                         .AsNoTracking(), pageNumber ?? 1, pageSize));
    }


    /// <summary>
    /// 直接新增一項商品，代入預設值
    /// </summary>
    /// <returns></returns>
    /// ProductManagement/Create
    [HttpGet]
    public IActionResult Create()
    {
        try
        {
            ProductStyle style = new ProductStyle()
            {
                Name = "基本款",
                Price = 0,
                Stock = 0
            };

            Product product = new Product()
            {
                Name = "新項目",
                Status = ProductStatus.Draft,
                CategoryId = _context.Category.First().Id,
                ProductStyles = new List<ProductStyle> { style },
                PublishTime = DateTime.Now // 預設上架時間為當前時間
            };

            _context.Add(product);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            TempData["message"] = string.Format("Could not add new item.");
            // 考慮將異常記錄到日誌以便進一步調查問題
            // _logger.LogError(ex, "Failed to create product");
        }
        return RedirectToAction("Index");
    }

    /// <summary>
    /// 編輯商品頁面
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// ProductManagement/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Product == null)
        {
            return NotFound();
        }

        var product = await _context.Product.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        MapperConfiguration config = new MapperConfiguration(cfg =>
            cfg.CreateMap<Product, ProductIndexViewModel>());

        // Using automapper
        Mapper mapper = new Mapper(config);
        ProductIndexViewModel viewModel = mapper.Map<ProductIndexViewModel>(product);
        viewModel.ProductId = product.Id;
        viewModel.Categories = new SelectList(_context.Set<Category>(), "Id", "Name", product.CategoryId);

        // 再從db撈 當前 productId的 detail圖片
        var imageList =
            _context.ProductImage.Where(x => x.ProductId == product.Id && x.Type == ProductImageType.Detail);
        viewModel.ProductImage = imageList.OrderBy(x => x.Sequence).ToList();

        // 商品狀態
        viewModel.Statuses = new SelectList(_productStatusEnumList, "Value", "Text", product.Status);

        // 商品款式
        var productStyles =
            _context.ProductStyle.Where(x => x.ProductId == product.Id).ToList();
        viewModel.ProductStyles = productStyles;

        return View(viewModel);
    }

    /// <summary>
    /// 商品 存檔事件
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// POST: ProductManagement/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(ProductEditDto dto)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // 確認是否有 db 資料
                var model = await _context.Product.FindAsync(dto.ProductId);
                if (model == null)
                {
                    return NotFound();
                }

                // 指定欄位map
                model.Name = dto.Name;
                model.Description = dto.Description;
                model.Promotion = dto.Promotion ?? string.Empty;
                model.Content = dto.Content;
                // 如果當前狀態為售完，但是款式數量>0，則商品狀態改為 販售中
                model.Status = (dto.Status == ProductStatus.Sold && dto.ProductStyles.Any(x => x.Stock > 0)) ? ProductStatus.Active : dto.Status;
                model.CategoryId = dto.CategoryId;
                model.ManufacturingMethod = dto.ManufacturingMethod;
                model.ManufacturingTime = dto.ManufacturingTime;
                model.ManufacturingCustomDate = dto.ManufacturingCustomDate;
                model.PublishTime = dto.PublishTime;
                // 更新 商品 資料
                _context.Update(model);

                // 更新 款式 資料
                // 從資料庫中取得相同pid的資料
                var productStylesFromDb = await _context.ProductStyle.Where(m => m.ProductId == model.Id).ToListAsync();

                MapperConfiguration productStyleConfig = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ProductStyleDto, ProductStyle>());
                Mapper productStyleMapper = new Mapper(productStyleConfig);
                var productStylesFromFrontEnd = productStyleMapper.Map<List<ProductStyle>>(dto.ProductStyles);
                productStylesFromFrontEnd.RemoveAll(m => string.IsNullOrWhiteSpace(m.Name));

                var productStylesToDelete = productStylesFromDb.Where(m => !productStylesFromFrontEnd.Any(f => f.Id == m.Id)).ToList();
                if (productStylesToDelete?.Any() ?? false)
                {
                    _context.ProductStyle.RemoveRange(productStylesToDelete);
                }

                foreach (var item in productStylesFromFrontEnd)
                {
                    var existingProductStyle = productStylesFromDb.FirstOrDefault(x => x.Id == item.Id);
                    if (existingProductStyle == null)
                    {
                        ProductStyle newProductStyle = new ProductStyle
                        {
                            ProductId = model.Id,
                            Name = item.Name,
                            Price = item.Price,
                            Stock = item.Stock
                        };
                        _context.ProductStyle.Add(newProductStyle);
                    }
                    else
                    {
                        existingProductStyle.ProductId = model.Id;
                        existingProductStyle.Name = item.Name;
                        existingProductStyle.Price = item.Price;
                        existingProductStyle.Stock = item.Stock;
                        _context.ProductStyle.Update(existingProductStyle);
                    }
                }
                // 商品款式至少要有一項，前端也要有防呆
                if (_context.ProductStyle.Count() == 0)
                {
                    _context.ProductStyle.Add(new ProductStyle()
                    {
                        Name = "基本款",
                        Price = 0,
                        Stock = 0
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {

                if (!(_context.Product?.Any(e => e.Id == dto.ProductId)).GetValueOrDefault())
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        return RedirectToAction(nameof(Edit), new { id = dto.ProductId });
    }


    // GET: ProductManagement/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Product == null)
        {
            return NotFound();
        }

        var product = await _context.Product.Include(p => p.Category).AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // POST: ProductManagement/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Product == null)
        {
            return Problem("Entity set 'OnlineShopContext.Product'  is null.");
        }
        var product = await _context.Product.FirstOrDefaultAsync(p => p.Id == id);
        if (product != null)
        {
            var history = new ProductHistory()
            {
                ProductId = product.Id,
                Name = product.Name,
                Description = product.Description,
                Promotion = product.Promotion,
                Content = product.Content,
                Status = product.Status,
                ManufacturingMethod = product.ManufacturingMethod,
                ManufacturingTime = product.ManufacturingTime,
                ManufacturingCustomDate = product.ManufacturingCustomDate,
                PublishTime = product.PublishTime,
                CategoryId = product.CategoryId,
                DeleteTime = DateTime.UtcNow.AddHours(8)
            };
            _context.ProductHistory.Add(history);

            _context.Product.Remove(product);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// 商品 詳細資料
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// ProductManagement/Details/5
    [HttpGet]
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
}
