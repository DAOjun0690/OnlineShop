using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Core.Dto;

public class OrderCreateDto
{
    /// <summary>
    /// 付款者ID
    /// </summary>
    [MaxLength(200)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 付款者帳號
    /// </summary>
    [MaxLength(200)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 送貨地點
    /// </summary>
    public int SelectedDeliveryAddress { get; set; }
    /// <summary>
    /// 送貨方式
    /// </summary>
    public int SelectedDeliveryMethod { get; set; }

    /// <summary>
    /// 收件人姓名
    /// </summary>
    [Required(ErrorMessage = "收件人姓名 是必填項目")]
    [MaxLength(200)]
    public string ReceiverName { get; set; }

    /// <summary>
    /// 收件人地址 1
    /// </summary>
    [Required(ErrorMessage = "地址 是必填項目")]
    [MaxLength(2000)]
    public string ReceiverFirstAddress { get; set; }

    /// <summary>
    /// 收件人地址 2
    /// </summary>
    [Required(ErrorMessage = "地址 是必填項目")]
    [MaxLength(2000)]
    public string ReceiverSecondAddress { get; set; }

    /// <summary>
    /// 收件人電話
    /// </summary>
    [Required(ErrorMessage = "收件人電話 是必填項目")]
    [MaxLength(200)]
    public string ReceiverPhone { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [MaxLength(200)]
    public string? Note { get; set; }
}
