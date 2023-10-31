using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Core.Models;

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
    [MaxLength(200)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 付款者帳號
    /// </summary>
    [MaxLength(200)]
    public string UserName { get; set; } = string.Empty;
    public int Total { get; set; }

    public int SelectedDeliveryAddress { get; set; }

    /// <summary>
    /// 送貨地點
    /// 1-中華郵政(含i郵箱/存局候領)
    /// 2-7-11
    /// 3-全家
    /// 4,5-順豐速運
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

    /// <summary>
    /// 付款狀態
    /// </summary>
    public bool isPaid { get; set; }

    /// <summary>
    /// 出貨狀態
    /// </summary>
    public bool isShip { get; set; }

    /// <summary>
    /// 訂單內容
    /// </summary>
    public List<OrderItem> OrderItem { get; set; }
}