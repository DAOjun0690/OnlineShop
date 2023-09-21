using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Core.Models;

public class OnlineShopUserRole
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string RoleName { get; set; }
}
