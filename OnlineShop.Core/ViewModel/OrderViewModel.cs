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
}
