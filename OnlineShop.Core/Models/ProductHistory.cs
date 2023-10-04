using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Core.Models;


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
    [MaxLength(200)]
    public string Name { get; set; }
    /// <summary>
    /// 商品簡介
    /// </summary>
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// 商品促銷
    /// </summary>
    [MaxLength(5000)]
    public string Promotion { get; set; } = string.Empty;
    /// <summary>
    /// 商品描述
    /// </summary>
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;

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

    /// <summary>
    /// 類別 (Foreign Key)
    /// </summary>
    public int CategoryId { get; set; }
    /// <summary>
    /// 刪除日期
    /// </summary>
    public DateTimeOffset DeleteTime { get; set; }
}
