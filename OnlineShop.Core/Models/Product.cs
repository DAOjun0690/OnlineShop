using System.ComponentModel;
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
    /// <summary>
    /// 商品圖片
    /// </summary>
    public IList<ProductImage> Image { get; set; }

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
    [Description("草稿")]
    Draft = 1,

    [Description("販售中")]
    Active = 2,

    [Description("售完")]
    Sold = 3
}

/// <summary>
/// 製造方式
/// </summary>
public enum ManufacturingMethod
{
    [Description("手工製造")]
    Manual,

    [Description("機器製造")]
    Machine
}

/// <summary>
/// 製造方式
/// </summary>
public enum ManufacturingTime
{
    [Description("現貨")]
    InStock,

    [Description("接單訂製")]
    Custom
}

