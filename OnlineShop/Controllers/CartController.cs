using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Core.Dto;
using OnlineShop.Data;
using OnlineShop.Helpers;
using OnlineShop.Core.Models;
using OnlineShop.Core.ViewModel;

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
        CartIndexViewModel viewModel = new CartIndexViewModel()
        {
            CartItems = CartItems,
            DeliveryAddresses = new List<DeliveryAddress>
            {
                new DeliveryAddress { Id = 1, Name = "臺灣" },
                new DeliveryAddress { Id = 2, Name = "中國" },
                new DeliveryAddress { Id = 3, Name = "香港" },
            },
            DeliveryMethods = new List<DeliveryMethod>
            {
                new DeliveryMethod { Id = 1, Name = "中華郵政", Price = 60, AddressId = 1 },
                new DeliveryMethod { Id = 2, Name = "7-11", Price = 60, AddressId = 1 },
                new DeliveryMethod { Id = 3, Name = "全家", Price = 60, AddressId = 1 },
                new DeliveryMethod { Id = 4, Name = "順豐(貨到付款)", Price = 0, AddressId = 2 },
                new DeliveryMethod { Id = 5, Name = "順豐(貨到付款)", Price = 0, AddressId = 3 },
            }
        };

        return View(viewModel);
    }

    /// <summary>
    /// 加入購物車
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public IActionResult AddtoCart(ShopCartDto dto)
    {
        if (dto.Amount == 0)
        {
            return Json(new { success = false, message = "購買數量不可為零。" });
        }

        int id = dto.ProductId;

        // 取得商品資料
        var product = _context.Product
            .Include(p => p.ProductStyles).AsNoTracking()
            .Single(x => x.Id.Equals(id));
        if (product == null) return NoContent();
        //if (product.Status != ProductStatus.Active) 
        //{
        //    return Json(new { success = false, message = "當前商品不開放購買，請在挑選其他項商品。" });
        //}

        var productStyle =
            product.ProductStyles.SingleOrDefault(x => x.Id == dto.ProductStyleId);
        if (productStyle == null) return NoContent();

        // 款式 庫存數量驗證
        if (dto.Amount > productStyle.Stock)
        {
            return Json(new { success = false, message = "購買數量超過庫存，請調整購買數量。" });
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
                // 款式 庫存數量驗證
                if (cart[index].Amount + item.Amount > productStyle.Stock)
                {
                    return Json(new { success = false, message = "購買數量超過庫存，請調整購買數量。" });
                }

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
