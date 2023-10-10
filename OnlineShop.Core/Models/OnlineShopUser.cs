using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Core.Models;

// Add profile data for application users by adding properties to the OnlineShopUser class
public class OnlineShopUser : IdentityUser
{
    /// <summary>
    /// 名稱
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; }
    /// <summary>
    /// 生日
    /// </summary>
    public DateTime DOB { get; set; }
    /// <summary>
    /// 性別
    /// </summary>
    public GenderType Gender { get; set; }
    /// <summary>
    /// 註冊日期
    /// </summary>
    public DateTime RegistrationDate { get; set; }
}

public enum GenderType
{
    [Description("男")]
    Male,
    [Description("女")]
    Female,
    [Description("不便透露")]
    Unknown
}

