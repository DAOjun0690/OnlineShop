using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models;

/// <summary>
/// 用於 商品 新增的viewmodel
/// </summary>
public class ProductIndexViewModel
{
    [Required(ErrorMessage = "名稱 是必填項目")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 商品簡介
    /// </summary>
    [Required(ErrorMessage = "簡介 是必填項目")]
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// 商品促銷
    /// </summary>
    public string Promotion { get; set; } = string.Empty;
    /// <summary>
    /// 商品描述
    /// </summary>
    public string Content { get; set; } = string.Empty;
    /// <summary>
    /// 商品價格
    /// </summary>
    public int Price { get; set; }
    /// <summary>
    /// 商品庫存
    /// </summary>
    public int Stock { get; set; }

    public byte[] Image { get; set; }

    public int CategoryId { get; set; }

    public SelectList Categories { get; set; }

    public IList<ProductStyle> ProductStyles { get; set; }
}
