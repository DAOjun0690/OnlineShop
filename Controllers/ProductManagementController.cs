using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using OnlineShop.Data;
using OnlineShop.Models;
using System;
using static StackExchange.Redis.Role;

namespace OnlineShop.Controllers;

//[Authorize(Roles = "Administrator")]
public class ProductManagementController : Controller
{
    private readonly OnlineShopContext _context;

    public ProductManagementController(OnlineShopContext context)
    {
        _context = context;
    }

    // GET: ProductManagement
    public async Task<IActionResult> Index(string searchString,
                                           string currentFilter,
                                           int? pageNumber,
                                           ProductStatus status = 0
                                           )
    {
        // 初始化頁碼
        if (searchString != null)
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }
        // 儲存當前搜尋狀態
        ViewData["CurrentFilter"] = searchString;

        var enumValues = Enum.GetValues(typeof(ProductStatus))
                             .Cast<ProductStatus>()
                             .Select(e => new SelectListItem
                             {
                                 Value = ((int)e).ToString(),
                                 Text = e.ToString()
                             })
                             .ToList();
        ViewData["StatusList"] = new SelectList(enumValues, "Value", "Text");

        var result = from m in _context.Product select m;

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            result = result.Where(s => s.Name.Contains(searchString));
        }
        // 判斷 產品狀態
        if (status != 0)
        {
            result = result.Where(p => p.Status == status);
        }

        //一頁顯示幾項
        int pageSize = 5;
        return View(await PaginatedList<Product>.CreateAsync(
            result.Include(p => p.Category).AsNoTracking(), pageNumber ?? 1, pageSize));
    }


    /// <summary>
    /// ProductManagement/Create
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Create()
    {
        Product product;
        try
        {
            product = new Product()
            {
                Name = "新項目",
                Status = ProductStatus.Draft,
                CategoryId = _context.Category.First().Id
            };
            _context.Add(product);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            TempData["message"] = string.Format("Could not add new item.");
        }
        return RedirectToAction("Index");
    }

    // GET: ProductManagement/Edit/5
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

        //Using automapper
        Mapper mapper = new Mapper(config);
        ProductIndexViewModel viewModel = mapper.Map<ProductIndexViewModel>(product);
        viewModel.ProductId = product.Id;
        viewModel.Categories = new SelectList(_context.Set<Category>(), "Id", "Name", product.CategoryId);
        // 再從db撈 當前 productId的 detail圖片
        var imageList =
            _context.ProductImage.Where(x => x.ProductId == product.Id && x.Type == ProductImageType.Detail);
        viewModel.ProductImage = imageList.ToList();

        // 案件狀態
        var enumValues = Enum.GetValues(typeof(ProductStatus))
                     .Cast<ProductStatus>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.ToString()
                     })
                     .ToList();
        viewModel.Statuses = new SelectList(enumValues, "Value", "Text", product.Status);

        var productStyles =
            _context.ProductStyle.Where(x => x.ProductId == product.Id).ToList();
        viewModel.ProductStyles = productStyles;

        return View(viewModel);
    }

    // POST: ProductManagement/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(ProductIndexViewModel dto)
    {
        if (ModelState.IsValid)
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
            model.Promotion = dto.Promotion;
            model.Content = dto.Content;
            model.Price = dto.Price;
            model.Stock = dto.Stock;
            model.Status = dto.Status;
            model.CategoryId = dto.CategoryId;

            // 編輯資料
            _context.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        //ViewData["Categories"] = new SelectList(
        //                            _context.Set<Category>(), "Id", "Name", product.CategoryId
        //                         );
        return View(nameof(Edit));
    }

    //---------------------------------------------------------------------------------------------
    // GET: ProductManagement/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        //if (id == null || _context.Product == null)
        //{
        //    return NotFound();
        //}

        ////建立一個 ViewModel
        //DetailViewModel dvm = new DetailViewModel();

        //var product = await _context.Product
        //    .Include(p => p.Category)
        //    .FirstOrDefaultAsync(m => m.Id == id);

        //if (product == null)
        //{
        //    return NotFound();
        //}
        //else
        //{
        //    dvm.product = product;
        //    if (product.Image != null)
        //    {
        //        //dvm.imgsrc = ViewImage(product.Image);
        //    }
        //}

        //return View(dvm);
        return View();
    }

    private string ViewImage(byte[] arrayImage)
    {
        // 二進位圖檔轉字串
        string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
        return "data:image/png;base64," + base64String;
    }

    // POST: ProductManagement/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product, IFormFile myimg)
    {
        var prod = new Product();

        if (id != product.Id)
        {
            return NotFound();
        }

        //if (ModelState.IsValid)
        {
            try
            {
                if (myimg != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        myimg.CopyTo(ms);
                        //product.Image = ms.ToArray();
                    }
                }
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: ProductManagement/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Product == null)
        {
            return NotFound();
        }

        var product = await _context.Product
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
        var product = await _context.Product.FindAsync(id);
        if (product != null)
        {
            _context.Product.Remove(product);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProductExists(int id)
    {
        return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    public IActionResult CreateCategory()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(Category category)
    {
        _context.Category.Add(category);
        await _context.SaveChangesAsync();
        return View();
    }

}
