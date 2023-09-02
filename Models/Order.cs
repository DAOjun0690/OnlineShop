using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models;

public class Order
{

    /// <summary>
    /// 訂單建立時間
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:000000}")]
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 付款者ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 付款者帳號
    /// </summary>
    public string UserName { get; set; }
    public int Total { get; set; }


    /// <summary>
    /// 收貨者姓名
    /// </summary>
    [Required]
    public string ReceiverName { get; set; }

    /// <summary>
    /// 收貨者地址
    /// </summary>
    [Required]
    public string ReceiverAdress { get; set; }

    /// <summary>
    /// 收貨者電話
    /// </summary>
    [Required]
    public string ReceiverPhone { get; set; }

    /// <summary>
    /// 付款狀態
    /// </summary>
    public bool isPaid { get; set; }

    /// <summary>
    /// 訂單內容
    /// </summary>
    public List<OrderItem> OrderItem { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
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
        this.Amount = order.Amount;
        this.SubTotal = order.SubTotal;
    }

    public Product Product { get; set; }
    public string imageSrc { get; set; }
}
