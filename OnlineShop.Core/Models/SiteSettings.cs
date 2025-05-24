using System.ComponentModel;

namespace OnlineShop.Core.Models;

/// <summary>
/// 網站設定
/// </summary>
public class SiteSettings
{
    public int Id { get; set; }

    /// <summary>
    /// 設定鍵名
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 設定值
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 設定描述
    /// </summary>
    public string Description { get; set; }
}

/// <summary>
/// 商品排序方式
/// </summary>
public enum ProductSortOrder
{
    /// <summary>
    /// 依 ID 降序排列（最新的在前）
    /// </summary>
    [Description("依 ID 降序排列（最新的在前）")]
    IdDesc = 1,

    /// <summary>
    /// 依 ID 升序排列（最舊的在前）
    /// </summary>
    [Description("依 ID 升序排列（最舊的在前）")]
    IdAsc = 2,

    /// <summary>
    /// 依上架時間降序排列（最新的在前）
    /// </summary>
    [Description("依上架時間降序排列（最新的在前）")]
    PublishTimeDesc = 3,

    /// <summary>
    /// 依上架時間升序排列（最舊的在前）
    /// </summary>
    [Description("依上架時間升序排列（最舊的在前）")]
    PublishTimeAsc = 4
} 