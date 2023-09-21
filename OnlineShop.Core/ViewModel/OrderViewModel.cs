using OnlineShop.Core.Models;

namespace OnlineShop.Core.ViewModel;

public class OrderViewModel
{
    public Order Order { get; set; }
    public List<CartItem> CartItems { get; set; }
}
