namespace OnlineShop.Models;

public class ProductStyle
{
    public int ProductId { get; set; }

    public int Id { get; set; }
    public string Name { get; set; }

    //導覽屬性
    public Product Products { get; set; }
}
