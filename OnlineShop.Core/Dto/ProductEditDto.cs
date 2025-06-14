﻿using OnlineShop.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Core.Dto;

public class ProductEditDto
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "名稱 是必填項目")]
    public string Name { get; set; }

    /// <summary>
    /// 商品簡介
    /// </summary>
    [Required(ErrorMessage = "簡介 是必填項目")]
    public string Description { get; set; }
    /// <summary>
    /// 商品促銷
    /// </summary>
    public string? Promotion { get; set; }
    /// <summary>
    /// 商品描述
    /// </summary>
    public string Content { get; set; }

    public ProductStatus Status { get; set; }

    /// <summary>
    /// 製造方式
    /// </summary>
    public ManufacturingMethod ManufacturingMethod { get; set; }
    /// <summary>
    /// 製作時間
    /// </summary>
    public ManufacturingTime ManufacturingTime { get; set; }
    /// <summary>
    /// 製作時間 - 接單訂製 需等待的時間
    /// </summary>
    public int ManufacturingCustomDate { get; set; }

    /// <summary>
    /// 上架時間
    /// </summary>
    public DateTime? PublishTime { get; set; }

    public int CategoryId { get; set; }

    public IList<ProductStyleDto> ProductStyles { get; set; }
}

public class ProductStyleDto
{
    /// <summary>
    /// 自己的identity
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 款式名稱
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 商品價格
    /// </summary>
    public int Price { get; set; }
    /// <summary>
    /// 商品庫存
    /// </summary>
    public int Stock { get; set; }
}

