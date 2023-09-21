namespace OnlineShop.Models;

/// <summary>
/// 使用者 前台顯示 list view model
/// </summary>
public class DetailViewModel
{
    /// <summary>
    /// 商品 流水編號
    /// </summary>
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    /// <summary>
    /// 商品顯示的第一張圖片
    /// </summary>
    public ProductImage Image { get; set; }

    /// <summary>
    /// 商品價格
    /// </summary>
    public int MinPrice { get; set; }

    public int MaxPrice { get; set; }

    /// <summary>
    /// 商品庫存
    /// </summary>
    public int Stock { get; set; }

    public ProductStatus Status { get; set; }
}
