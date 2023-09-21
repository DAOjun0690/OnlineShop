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


    /// <summary>
    /// 收貨者姓名
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string ReceiverName { get; set; }

    /// <summary>
    /// 收貨者地址
    /// </summary>
    [Required]
    [MaxLength(2000)]
    public string ReceiverAdress { get; set; }

    /// <summary>
    /// 收貨者電話
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string ReceiverPhone { get; set; }

    /// <summary>
    /// 付款狀態
    /// </summary>
    public bool isPaid { get; set; }

    /// <summary>
    /// 訂單內容
    /// </summary>
    public List<OrderItem> OrderItem { get; set; }
}