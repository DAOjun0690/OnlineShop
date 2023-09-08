using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

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
    public async Task<IActionResult> Index(string searchString, string currentFilter, int? pageNumber)
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

        var result = from m in _context.Product select m;

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            result = result.Where(s => s.Name.Contains(searchString));
        }

        //一頁顯示幾項
        int pageSize = 5;
        return View(await PaginatedList<Product>.CreateAsync(
            result.Include(p => p.Category).AsNoTracking(), pageNumber ?? 1, pageSize));
    }

    // GET: ProductManagement/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Product == null)
        {
            return NotFound();
        }

        //建立一個 ViewModel
        DetailViewModel dvm = new DetailViewModel();

        var product = await _context.Product
            .Include(p => p.Category)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (product == null)
        {
            return NotFound();
        }
        else
        {
            dvm.product = product;
            if (product.Image != null)
            {
                //dvm.imgsrc = ViewImage(product.Image);
            }
        }

        return View(dvm);
    }

    private string ViewImage(byte[] arrayImage)
    {
        // 二進位圖檔轉字串
        string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
        return "data:image/png;base64," + base64String;
    }

    /// <summary>
    /// ProductManagement/Create
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Create()
    {
        ProductIndexViewModel viewModel = new ProductIndexViewModel();
        viewModel.Title = "Create";
        viewModel.Categories = new SelectList(_context.Set<Category>(), "Id", "Name");
        viewModel.ProductImage = new List<ProductImage>();
        return View("CreateOrEditProduct", viewModel);
    }

    // POST: ProductManagement/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(ProductIndexViewModel dto, IFormFile myimg)
    {
        if (ModelState.IsValid)
        {
            //Initialize the mapper
            MapperConfiguration config = new MapperConfiguration(cfg =>
            cfg.CreateMap<ProductIndexViewModel, Product>());

            //Using automapper
            Mapper mapper = new Mapper(config);
            Product model = mapper.Map<Product>(dto);

            // 確認是否有 db 資料
            if (model.Id == 0) // 如果是新增資料
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
            }
            else // 編輯資料
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //ViewData["Categories"] = new SelectList(
        //                            _context.Set<Category>(), "Id", "Name", product.CategoryId
        //                         );
        return View(nameof(Edit));
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
        else
        {
            if (product.Image != null)
            {
                //ViewBag.Image = ViewImage(product.Image);
            }
        }
        var productStyles =
            _context.ProductStyle.Where(x => x.ProductId == product.Id).ToList();
        product.ProductStyles = productStyles;

        //設定seleccted項目
        ViewData["Categories"] = new SelectList(_context.Set<Category>(), "Id", "Name", product.CategoryId);

        return View(product);
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
