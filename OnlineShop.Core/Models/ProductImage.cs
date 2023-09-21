using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Core.Models;

/// <summary>
/// 產品 圖片
/// </summary>
public class ProductImage
{
    public int Id { get; set; }

    public Guid Guid { get; set; }
    public int ProductId { get; set; }
    public int Sequence { get; set; }
    [MaxLength(500)]
    public string FileName { get; set; }
    public ProductImageType Type { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }
}

/// <summary>
/// 商品狀態
/// </summary>
public enum ProductImageType
{
    /// <summary>
    /// 商品主要圖片
    /// </summary>
    Main,
    /// <summary>
    /// 商品介紹圖片
    /// </summary>
    Detail,
    /// <summary>
    /// 商品簡介圖片
    /// </summary>
    Content
}
