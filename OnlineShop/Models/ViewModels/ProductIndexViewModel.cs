﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models;

/// <summary>
/// 用於 商品 編輯的 View Model
/// </summary>
public class ProductIndexViewModel
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
    public string Promotion { get; set; }
    /// <summary>
    /// 商品描述
    /// </summary>
    public string Content { get; set; }

    public IList<ProductImage> ProductImage { get; set; }

    public ProductStatus Status { get; set; }

    public SelectList Statuses { get; set; }

    public int CategoryId { get; set; }

    public SelectList Categories { get; set; }

    public IList<ProductStyle> ProductStyles { get; set; }
}