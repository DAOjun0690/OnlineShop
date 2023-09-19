using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models;

public class ProductStyle
{
    /// <summary>
    /// 自己的identity
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 綁定的product id
    /// </summary>
    public int ProductId { get; set; }
    /// <summary>
    /// 款式名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 商品價格
    /// </summary>
    public int Price { get; set; }
    /// <summary>
    /// 商品庫存
    /// </summary>
    public int Stock { get; set; }

    //導覽屬性
    [ForeignKey("ProductId")]
    public virtual Product Products { get; set; }
}
