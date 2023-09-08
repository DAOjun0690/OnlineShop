using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models;

/// <summary>
/// 產品 圖片
/// </summary>
public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Sequence { get; set; }
    public string LinkToLargeImage { get; set; }
    public string LinkToMediumImage { get; set; }
    public string LinkToSmallImage { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }
}
