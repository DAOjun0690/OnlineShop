using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Core.Models;

public class Category
{
    public int Id { get; set; }
    /// <summary>
    /// 類別名稱
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; }

    public List<Product> Products { get; set; }
}
