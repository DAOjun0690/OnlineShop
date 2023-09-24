using OnlineShop.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Core.ViewModel;

/// <summary>
/// 用於 商品 詳細資料的 View Model
/// </summary>
public class ProductDetailViewModel
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "名稱 是必填項目")]
    public string Name { get; set; }

    /// <summary>
    /// 商品簡介
    /// </summary>
    [Required(ErrorMessage = "簡介 是必填項目")]
    public string Description { get; set; }
    /// <summary>
    /// 商品促銷
    /// </summary>
    public string Promotion { get; set; }
    /// <summary>
    /// 商品描述
    /// </summary>
    public string Content { get; set; }

    public IList<ProductImage> ProductImage { get; set; }

    public ProductStatus Status { get; set; }

    /// <summary>
    /// 製造方式
    /// </summary>
    public ManufacturingMethod ManufacturingMethod { get; set; }
    /// <summary>
    /// 製作時間
    /// </summary>
    public ManufacturingTime ManufacturingTime { get; set; }
    /// <summary>
    /// 製作時間 - 接單訂製 需等待的時間
    /// </summary>
    public int ManufacturingCustomDate { get; set; }

    public string CategoryName { get; set; }

    public IList<ProductStyle> ProductStylesList { get; set; }
}
