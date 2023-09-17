using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Helpers;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

public class OrderController : Controller
{
    private readonly OnlineShopContext _context;
    private readonly UserManager<OnlineShopUser> _userManager;

    public OrderController(OnlineShopContext context, UserManager<OnlineShopUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // 查詢分兩種 已登入User 跟 使用訂單編號查詢

    /// <summary>
    /// 我的訂單列表
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Index()
    {
        List<OrderViewModel> orderVM = new List<OrderViewModel>();

        var userId = _userManager.GetUserId(User);
        var orders = await _context.Order.
            OrderByDescending(k => k.OrderDate).            //用日期排序
            Where(m => m.UserId == userId).ToListAsync();   //取得屬於當前登入者的訂單

        foreach (var item in orders)
        {
            item.OrderItem = await _context.OrderItem.
                Where(p => p.OrderId == item.Id).ToListAsync(); //取得訂單內的商品項目

            var ovm = new OrderViewModel()
            {
                Order = item,
                CartItems = GetOrderItems(item.Id)
            };

            orderVM.Add(ovm);
        }

        return View(orderVM);
    }

    /// <summary>
    /// 訂單資訊
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public async Task<IActionResult> Details(int? Id)
    {
        if (Id == null)
        {
            return NotFound();
        }

        var order = await _context.Order.FirstOrDefaultAsync(m => m.Id == Id);
        if (order.UserId != _userManager.GetUserId(User))
        {
            return NotFound();
        }
        else
        {
            order.OrderItem = await _context.OrderItem.Where(p => p.OrderId == Id).ToListAsync();
            ViewBag.orderItems = GetOrderItems(order.Id);
        }

        return View(order);
    }

    /// <summary>
    /// 結帳
    /// </summary>
    /// <returns></returns>
    public IActionResult Checkout()
    {
        //確認 Session 內存在購物車
        if (SessionHelper.GetObjectFromJson<List<OrderItem>>(HttpContext.Session, "cart") == null)
        {
            return RedirectToAction("Index", "Cart");
        }

        //建立新的訂單
        var myOrder = new Order()
        {
            UserId = _userManager.GetUserId(User),
            UserName = _userManager.GetUserName(User),
            OrderItem = SessionHelper.GetObjectFromJson<List<OrderItem>>(HttpContext.Session, "cart")
        };
        myOrder.Total = myOrder.OrderItem.Sum(m => m.SubTotal);
        ViewBag.CartItems = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

        return View(myOrder);
    }

    /// <summary>
    /// 新增訂單到資料庫
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateOrder(Order order)
    {
        //新增訂單到資料庫
        if (ModelState.IsValid)
        {
            order.OrderDate = DateTime.Now;
            order.isPaid = false;
            order.OrderItem = SessionHelper.GetObjectFromJson<List<OrderItem>>(HttpContext.Session, "cart");

            _context.Add(order);
            await _context.SaveChangesAsync();
            SessionHelper.Remove(HttpContext.Session, "cart");

            return RedirectToAction("ReviewOrder", new { Id = order.Id });
        }
        return View();
    }

    /// <summary>
    /// 取得當前訂單
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public async Task<IActionResult> ReviewOrder(int? Id)
    {
        if (Id == null)
        {
            return NotFound();
        }

        var order = await _context.Order.FirstOrDefaultAsync(m => m.Id == Id);
        if (order.UserId != _userManager.GetUserId(User))
        {
            return NotFound();
        }
        else
        {
            order.OrderItem = await _context.OrderItem.Where(p => p.OrderId == Id).ToListAsync();
            ViewBag.orderItems = GetOrderItems(order.Id);
        }

        return View(order);
    }

    /// <summary>
    /// 取得商品詳細資料
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    private List<CartItem> GetOrderItems(int orderId)
    {

        var OrderItems = _context.OrderItem.Where(p => p.OrderId == orderId).ToList();

        List<CartItem> orderItems = new List<CartItem>();
        foreach (var orderitem in OrderItems)
        {
            CartItem item = new CartItem(orderitem);
            item.Product = _context.Product.Single(x => x.Id == orderitem.ProductId);
            orderItems.Add(item);
        }

        return orderItems;
    }
}
