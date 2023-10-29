using OnlineShop.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Core.ViewModel;

public class OnlineShopUserViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public OnlineShopUser User { get; set; }
    /// <summary>
    /// 註冊日期 (短日期)
    /// </summary>
    [DisplayFormat(DataFormatString = "{0:d}")]
    [Display(Name = "註冊日期")]
    public DateTime? RegistrationDate => User?.RegistrationDate;
    /// <summary>
    /// 角色名稱
    /// </summary>
    [Display(Name = "角色")]
    public string RoleName { get; set; }
}
