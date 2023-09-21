using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Core.Models;

public class Product
{
    public int Id { get; set; }
    /// <summary>
    /// 商品名稱
    /// </summary>
    [Required]
    public string Name { get; set; }
    /// <summary>
    /// 商品簡介
    /// </summary>
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
    /// 商品圖片
    /// </summary>
    public IList<ProductImage> Image { get; set; }

    public ProductStatus Status { get; set; }

    /// <summary>
    /// 類別 (Foreign Key)
    /// </summary>
    public int CategoryId { get; set; }
    // 導覽屬性
    [ForeignKey("CategoryId")]
    public Category Category { get; set; }

    /// <summary>
    /// 商品款式 延遲加載
    /// </summary>
    public virtual ICollection<ProductStyle> ProductStyles { get; set; }
}

/// <summary>
/// 商品狀態
/// </summary>
public enum ProductStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 1,
    /// <summary>
    /// 絕讚販售中
    /// </summary>
    Active = 2,
    /// <summary>
    /// 已售完
    /// </summary>
    Sold = 3
}