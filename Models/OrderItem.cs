namespace OnlineShop.Models;

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
    public string imageSrc { get; set; }
}
