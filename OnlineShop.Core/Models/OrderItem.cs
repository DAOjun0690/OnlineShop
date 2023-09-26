using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Core.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int ProductStyleId { get; set; }
    public int Amount { get; set; }
    public int SubTotal { get; set; }
}

public class CartItem : OrderItem
{
    public CartItem() { }
    public CartItem(OrderItem order)
    {
        this.OrderId = order.OrderId;
        this.ProductId = order.ProductId;
        this.ProductStyleId = order.ProductStyleId;
        this.Amount = order.Amount;
        this.SubTotal = order.SubTotal;
    }

    public Product Product { get; set; }
    public ProductStyle ProductStyle { get; set; }

    [MaxLength(200)]
    public string imageSrc { get; set; }

    public void IncreaseAmount(int amount)
    {
        Amount += amount;
        SubTotal += ProductStyle.Price * amount;
    }
}



/// <summary>
/// 送貨地點
/// </summary>
public class DeliveryAddress
{
    public int Id { get; set; }
    public string Name { get; set; }
}

/// <summary>
/// 送貨方式
/// </summary>
public class DeliveryMethod
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public int AddressId { get; set; } // 這個屬性表示該送貨方式對應的送貨地點
}
