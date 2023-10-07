using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Core.Models;
using OnlineShop.Data;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly OnlineShopContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="context"></param>
    public CategoryController(OnlineShopContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 取得 管理者 類別列表 資料
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Index()
    {
        return View(_context.Category.AsQueryable());
    }

    /// <summary>
    /// 新增類別
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Create()
    {
        try
        {
            Category category = new Category()
            {
                Name = "新類別"
            };

            _context.Add(category);
            _context.SaveChanges();
        }
        catch (Exception)
        {
            TempData["message"] = string.Format("無法新增類別!");
            // 考慮將異常記錄到日誌以便進一步調查問題
            // _logger.LogError(ex, "Failed to create category");
        }
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// 編輯類別 頁面
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Category.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    /// <summary>
    /// 編輯類別 事件
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Category model)
    {
        if (!string.IsNullOrWhiteSpace(model.Name))
        {
            try
            {
                // 檢查 DB 有無資料
                var category = await _context.Category.FindAsync(model.Id);
                if (category == null)
                {
                    return NotFound();
                }

                category.Name = model.Name;
                _context.Category.Update(category);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(_context.Category?.Any(c => c.Id == model.Id)).GetValueOrDefault())
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// 刪除類別 頁面
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Category.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    /// <summary>
    /// 刪除類別 事件
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Category == null)
        {
            return Problem("Entity set 'OnlineShopContext.Category'  is null.");
        }
        var category = await _context.Category.FirstOrDefaultAsync(p => p.Id == id);
        if (category != null)
        {
            _context.Category.Remove(category);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
