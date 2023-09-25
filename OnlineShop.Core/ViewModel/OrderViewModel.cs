using OnlineShop.Core.Models;

namespace OnlineShop.Core.ViewModel;

public class OrderViewModel
{
    public Order Order { get; set; }
    public List<CartItem> CartItems { get; set; }
    /// <summary>
    /// 送貨地點
    /// </summary>
    public string DeliveryAddressName { get; set; } = string.Empty;
    /// <summary>
    /// 送貨方式
    /// </summary>
    public string DeliveryMethodName { get; set; } = string.Empty;

    /// <summary>
    /// 如果是從 下訂單來的 訂單檢視 會是 1
    /// 0: 訂單查詢 1:送出訂單當下的訂單檢視
    /// </summary>
    public int fromReview { get; set; }
}
