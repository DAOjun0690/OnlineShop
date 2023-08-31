namespace OnlineShop.Models;

public class Product
{
    public int Id { get; set; }
    /// <summary>
    /// 商品名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 商品簡介
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 商品內容
    /// </summary>
    public string Content { get; set; }
    /// <summary>
    /// 商品價格
    /// </summary>
    public int Price { get; set; }
    /// <summary>
    /// 商品庫存
    /// </summary>
    public int Stock { get; set; }
    /// <summary>
    /// 商品圖片
    /// </summary>
    public byte[] Image { get; set; }
    /// <summary>
    /// 類別 (Foreign Key)
    /// </summary>
    public int CategoryId { get; set; }

    public Category Category { get; set; }
}
