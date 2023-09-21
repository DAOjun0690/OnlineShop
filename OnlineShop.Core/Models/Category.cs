namespace OnlineShop.Core.Models;

public class Category
{
    public int Id { get; set; }
    /// <summary>
    /// 類別名稱
    /// </summary>
    public string Name { get; set; }

    public List<Product> Products { get; set; }
}
