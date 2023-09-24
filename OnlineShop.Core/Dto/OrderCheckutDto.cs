using OnlineShop.Core.ViewModel;

namespace OnlineShop.Core.Dto;

public class OrderCheckutDto
{
    public int SelectedDeliveryAddress { get; set; }
    public int SelectedDeliveryMethod { get; set; }
}
