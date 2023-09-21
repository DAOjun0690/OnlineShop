using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models;


/// <summary>
/// 單純記錄刪除歷史
/// </summary>
public class ProductHistory
{
    public int Id { get; set; }
    public int ProductId { get; set; }
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

    public ProductStatus Status { get; set; }

    /// <summary>
    /// 類別 (Foreign Key)
    /// </summary>
    public int CategoryId { get; set; }
    /// <summary>
    /// 刪除日期
    /// </summary>
    public DateTimeOffset DeleteTime { get; set; }
}
