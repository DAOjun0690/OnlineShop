using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models;

public class ProductStyle
{
    /// <summary>
    /// 綁定的product id
    /// </summary>
    public int ProductId { get; set; }
    /// <summary>
    /// 自己的identity
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 款式名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    //導覽屬性
    [ForeignKey("ProductId")]
    public Product Products { get; set; }
}
