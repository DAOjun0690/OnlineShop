using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Helpers;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

/// <summary>
/// 訂單事件
/// </summary>
public class OrderController : Controller
{
    private readonly OnlineShopContext _context;
    private readonly UserManager<OnlineShopUser> _userManager;

    public OrderController(OnlineShopContext context,
                           UserManager<OnlineShopUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// 已登入user: 我的訂單列表
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
        // 取得訂單物件
        var order = await _context.Order.FirstOrDefaultAsync(m => m.Id == Id);

        // 如果找不到訂單，返回NotFound結果
        if (order == null)
        {
            return NotFound();
        }

        // 取得當前使用者物件
        var user = await _userManager.GetUserAsync(User);

        // 判斷當前使用者是否可以查閱訂單
        switch (order.UserId)
        {
            // 如果訂單沒有使用者Id，表示是未登入者所提繳的訂單
            case null:
                // 如果當前使用者也沒有登入，則可以查閱
                if (user == null)
                {
                    break;
                }
                // 否則，返回NotFound結果
                else
                {
                    return NotFound();
                }
            // 如果訂單有使用者Id，表示是已登入者所提繳的訂單
            default:
                // 如果當前使用者也有登入，且Id與訂單相同，或是屬於Admin角色，則可以查閱
                if (user != null && (order.UserId == user.Id || await _userManager.IsInRoleAsync(user, "Admin")))
                {
                    break;
                }
                // 否則，返回NotFound結果
                else
                {
                    return NotFound();
                }
        }

        // 取得訂單項目物件
        order.OrderItem = await _context.OrderItem.Where(p => p.OrderId == Id).ToListAsync();

        // 取得商品詳細資料
        var orderItems = GetOrderItems(order.Id);

        OrderViewModel viewModel = new OrderViewModel()
        {
            Order = order,
            CartItems = orderItems
        };

        // 返回View結果
        return View(viewModel);
    }

    /// <summary>
    /// 列出所有表單
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> OrderList()
    {
        List<OrderViewModel> orderVM = new List<OrderViewModel>();

        var userId = _userManager.GetUserId(User);
        var orders = await _context.Order.
            OrderByDescending(k => k.OrderDate).ToListAsync();           //用日期排序
                                                                         //Where(m => m.UserId == userId).ToListAsync();   //取得屬於當前登入者的訂單

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

    [HttpGet]
    public IActionResult QueryOrder()
    {
        return View();
    }

    /// <summary>
    /// 查詢是否有相同編號的訂單
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult QueryOrder(int? OrderId)
    {
        return RedirectToAction("Details", new { Id = OrderId });
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
        return View("Error");
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
        if (order.UserId != (_userManager.GetUserId(User) ?? ""))
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
