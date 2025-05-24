using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Models;
using OnlineShop.Services;

namespace OnlineShop.ViewComponents;

public class ProductSortOrderViewComponent : ViewComponent
{
    private readonly SiteSettingsService _siteSettingsService;

    public ProductSortOrderViewComponent(SiteSettingsService siteSettingsService)
    {
        _siteSettingsService = siteSettingsService;
    }

    public IViewComponentResult Invoke(bool isEditMode = false)
    {
        var sortOrder = _siteSettingsService.GetProductSortOrder();
        var sortOrderText = sortOrder switch
        {
            ProductSortOrder.IdAsc => "ID 升序 (舊→新)",
            ProductSortOrder.IdDesc => "ID 降序 (新→舊)",
            ProductSortOrder.PublishTimeAsc => "上架時間升序 (舊→新)",
            ProductSortOrder.PublishTimeDesc => "上架時間降序 (新→舊)",
            _ => "ID 降序 (新→舊)"
        };

        ViewBag.SortOrderText = sortOrderText;
        ViewBag.IsEditMode = isEditMode;

        return View();
    }
}
