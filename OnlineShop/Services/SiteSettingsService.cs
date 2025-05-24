using OnlineShop.Core.Models;
using OnlineShop.Data;

namespace OnlineShop.Services;

/// <summary>
/// 網站設定服務
/// </summary>
public class SiteSettingsService
{
    private readonly OnlineShopContext _context;

    /// <summary>
    /// 商品排序方式的設定鍵
    /// </summary>
    public const string PRODUCT_SORT_ORDER_KEY = "ProductSortOrder";

    public SiteSettingsService(OnlineShopContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 獲取設定值
    /// </summary>
    /// <param name="key">設定鍵</param>
    /// <returns>設定值</returns>
    public string GetSetting(string key)
    {
        var setting = _context.SiteSettings.FirstOrDefault(s => s.Key == key);
        return setting?.Value;
    }

    /// <summary>
    /// 設置設定值
    /// </summary>
    /// <param name="key">設定鍵</param>
    /// <param name="value">設定值</param>
    /// <param name="description">設定描述</param>
    public void SetSetting(string key, string value, string description = null)
    {
        var setting = _context.SiteSettings.FirstOrDefault(s => s.Key == key);
        if (setting == null)
        {
            setting = new SiteSettings
            {
                Key = key,
                Value = value,
                Description = description ?? string.Empty
            };
            _context.SiteSettings.Add(setting);
        }
        else
        {
            setting.Value = value;
            if (!string.IsNullOrEmpty(description))
            {
                setting.Description = description;
            }
            _context.SiteSettings.Update(setting);
        }
        _context.SaveChanges();
    }

    /// <summary>
    /// 獲取商品排序方式
    /// </summary>
    /// <returns>商品排序方式</returns>
    public ProductSortOrder GetProductSortOrder()
    {
        var value = GetSetting(PRODUCT_SORT_ORDER_KEY);
        if (string.IsNullOrEmpty(value) || !Enum.TryParse<ProductSortOrder>(value, out var result))
        {
            return ProductSortOrder.IdDesc; // 預設排序方式
        }
        return result;
    }

    /// <summary>
    /// 設置商品排序方式
    /// </summary>
    /// <param name="sortOrder">商品排序方式</param>
    public void SetProductSortOrder(ProductSortOrder sortOrder)
    {
        SetSetting(PRODUCT_SORT_ORDER_KEY, sortOrder.ToString(), "商品排序方式");
    }
} 