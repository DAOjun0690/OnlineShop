using OnlineShop.Core.Models;

namespace OnlineShop.Core.ViewModel;

public class CartIndexViewModel
{
    public List<CartItem> CartItems { get; set; }

    /// <summary>
    /// 送貨地點
    /// </summary>
    public List<DeliveryAddress> DeliveryAddresses { get; set; }
    public DeliveryAddress SelectedDeliveryAddress { get; set; }

    /// <summary>
    /// 送貨方式
    /// </summary>
    public List<DeliveryMethod> DeliveryMethods { get; set; }

    public DeliveryMethod SelectedDeliveryMethod { get; set; }

    public int Total
    {
        get
        {
            if (this.CartItems != null) { return CartItems.Sum(m => m.SubTotal); }
            else { return 0; }
        }
    }
}
