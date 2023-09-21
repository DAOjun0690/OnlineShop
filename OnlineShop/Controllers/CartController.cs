using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Core.Dto;
using OnlineShop.Data;
using OnlineShop.Helpers;
using OnlineShop.Core.Models;

namespace OnlineShop.Controllers;

public class CartController : Controller
{
    private readonly OnlineShopContext _context;

    public CartController(OnlineShopContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        List<CartItem> CartItems = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

        if (CartItems != null)
        {
            // 計算商品總額
            ViewBag.Total = CartItems.Sum(m => m.SubTotal);
        }
        else
        {
            ViewBag.Total = 0;
        }

        return View(CartItems);
    }

    /// <summary>
    /// 加入購物車
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public IActionResult AddtoCart(ShopCartDto dto)
    {
        int id = dto.ProductId;

        // 取得商品資料
        var product = _context.Product
            .Include(p => p.ProductStyles).AsNoTracking()
            .Single(x => x.Id.Equals(id));
        if (product == null)
        {
            return NoContent();
        }
        var productStyle =
            product.ProductStyles.SingleOrDefault(x => x.Id == dto.ProductStyleId);
        if (productStyle == null)
        {
            return NoContent();
        }
        CartItem item = CreateCartItem(product, productStyle, dto.Amount);

        // 判斷 Session 內有無購物車
        if (SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart") == null)
        {
            //如果沒有已存在購物車: 建立新的購物車
            List<CartItem> cart = new List<CartItem>
            {
                item
            };
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
        }
        else
        {
            //如果已存在購物車: 檢查有無相同的商品，有的話只調整數量
            List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            // FindIndex查詢位置
            int index = cart.FindIndex(m => m.Product.Id.Equals(id) && m.ProductStyleId == dto.ProductStyleId);

            if (index != -1)
            {
                cart[index].Amount += item.Amount;
                cart[index].SubTotal += item.SubTotal;
            }
            else
            {
                cart.Add(item);
            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
        }
        // HttpStatus 204: 請求成功但不更新畫面
        return NoContent();
    }

    public IActionResult RemoveItem(int id, int ProductStyleId)
    {
        // 向 Session 取得商品列表
        List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
        // 用FindIndex查詢目標在List裡的位置
        int index = cart.FindIndex(m => m.Product.Id.Equals(id) && m.ProductStyleId == ProductStyleId);
        cart.RemoveAt(index);

        if (cart.Count < 1)
        {
            SessionHelper.Remove(HttpContext.Session, "cart");
        }
        else
        {
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
        }

        return RedirectToAction("Index");
    }

    private string ViewImage(byte[] arrayImage)
    {
        string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
        return "data:image/png;base64," + base64String;
    }

    private CartItem CreateCartItem(Product product, ProductStyle productStyle, int amount)
    {
        return new CartItem()
        {
            Product = product,
            ProductStyle = productStyle,
            ProductId = product.Id,
            ProductStyleId = productStyle.Id,
            Amount = amount,
            SubTotal = productStyle.Price * amount,
            //imageSrc = ViewImage(product.Image) 或許可以使用picture/Download
        };
    }
}
