using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Helpers;
using OnlineShop.Core.Models;
using OnlineShop.Core.ViewModel;
using OnlineShop.Core.Dto;
using Microsoft.AspNetCore.Authorization;
using System.Web;
using AutoMapper;
using System.Text;

namespace OnlineShop.Controllers;

/// <summary>
/// 訂單事件 只有登入者可以進行操作
/// </summary>
[Authorize]
public class OrderController : Controller
{
    private readonly OnlineShopContext _context;
    private readonly UserManager<OnlineShopUser> _userManager;

    private readonly List<DeliveryAddress> _DeliveryAddresses = new List<DeliveryAddress>
            {
                new DeliveryAddress { Id = 1, Name = "臺灣" },
                new DeliveryAddress { Id = 2, Name = "中國" },
                new DeliveryAddress { Id = 3, Name = "香港" },
            };
    private readonly List<DeliveryMethod> _DeliveryMethods = new List<DeliveryMethod>
            {
                new DeliveryMethod { Id = 1, Name = "中華郵政", Price = 60, AddressId = 1 },
                new DeliveryMethod { Id = 2, Name = "7-11", Price = 60, AddressId = 1 },
                new DeliveryMethod { Id = 3, Name = "全家", Price = 60, AddressId = 1 },
                new DeliveryMethod { Id = 4, Name = "順豐速運(貨到付款)", Price = 0, AddressId = 2 },
                new DeliveryMethod { Id = 5, Name = "順豐速運(貨到付款)", Price = 0, AddressId = 3 },
            };

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
    public async Task<IActionResult> Details(int? Id, int fromReview)
    {
        // 取得訂單物件
        var order = await _context.Order.FirstOrDefaultAsync(m => m.Id == Id);

        // 如果找不到訂單，返回NotFound結果
        if (order == null)
        {
            return View(new OrderViewModel());
        }

        // 取得當前使用者物件
        var user = await _userManager.GetUserAsync(User);

        // 判斷當前使用者是否可以查閱訂單
        if (!CanUserViewOrder(order, user))
        {
            return View(new OrderViewModel());
        }

        // 取得訂單項目物件
        order.OrderItem = await _context.OrderItem.Where(p => p.OrderId == Id).ToListAsync();
        order.Total = order.OrderItem.Sum(m => m.SubTotal) +
            _DeliveryMethods.First(x => x.Id == order.SelectedDeliveryMethod).Price;
        // 取得商品詳細資料
        var orderItems = GetOrderItems(order.Id);

        OrderViewModel viewModel = new OrderViewModel()
        {
            Order = order,
            CartItems = orderItems,
            DeliveryAddressName = _DeliveryAddresses.First(x => x.Id == order.SelectedDeliveryAddress).Name,
            DeliveryMethodName = _DeliveryMethods.First(x => x.Id == order.SelectedDeliveryMethod).Name,
            fromReview = fromReview
        };

        // 返回View結果
        return View(viewModel);
    }

    /// <summary>
    /// 判斷當前使用者是否可以查閱訂單
    /// </summary>
    /// <param name="order"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    private bool CanUserViewOrder(Order order, OnlineShopUser user)
    {
        bool isUserAdmin = user != null && _userManager.IsInRoleAsync(user, "Admin").Result;

        return isUserAdmin || user != null && order.UserId == user.Id;
    }

    /// <summary>
    /// 列出所有訂單
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> OrderList()
    {
        var userId = _userManager.GetUserId(User);
        var orderList = await _context.Order
            .OrderByDescending(k => k.OrderDate)
            .ToListAsync();

        List<OrderViewModel> orderVM = orderList
            .Select(order => new OrderViewModel
            {
                Order = order,
                CartItems = GetOrderItems(order.Id)
            })
            .Where(ovm => ovm.CartItems.Any())
            .ToList();

        return View(orderVM);
    }
    //public async Task<IActionResult> OrderList(string searchString,
    //                                       string currentFilter,
    //                                       int? pageNumber)
    //{
    //    // 初始化頁碼
    //    if (searchString != null)
    //    {
    //        pageNumber = 1;
    //    }
    //    else
    //    {
    //        searchString = currentFilter;
    //    }
    //    // 儲存當前搜尋狀態
    //    ViewData["CurrentFilter"] = searchString;

    //    List<OrderViewModel> orderVM = new List<OrderViewModel>();

    //    var productIds = _context.Product.Select(p => p.Id).ToList();
    //    var userId = _userManager.GetUserId(User);
    //    var orderList = await _context.Order
    //        .OrderByDescending(k => k.OrderDate)
    //        .ToListAsync();       //用日期排序
    //                              //Where(m => m.UserId == userId).ToListAsync();   //取得屬於當前登入者的訂單

    //    foreach (var item in orderList)
    //    {
    //        var orderItemList = await _context.OrderItem.
    //            Where(p => p.OrderId == item.Id && productIds.Contains(p.ProductId)).ToListAsync(); //取得訂單內的商品項目

    //        if (orderItemList?.Any() ?? false)
    //        {
    //            item.OrderItem = orderItemList;

    //            var ovm = new OrderViewModel()
    //            {
    //                Order = item,
    //                CartItems = GetOrderItems(item.Id)
    //            };

    //            orderVM.Add(ovm);
    //        }
    //    }

    //    // 商品名稱 模糊查詢
    //    if (!string.IsNullOrWhiteSpace(searchString))
    //    {
    //        orderVM = orderVM.Where(s => s.Order.UserName.Contains(searchString)).ToList();
    //    }

    //    // 預設一頁顯示幾項
    //    int pageSize = 5;

    //    return View(await PaginatedList<OrderViewModel>.CreateAsync(
    //        orderVM.AsQueryable(), pageNumber ?? 1, pageSize));
    //}


    /// <summary>
    /// 更改訂單狀態
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> StatusChange(int id, string code)
    {
        string encodeCode = HttpUtility.HtmlEncode(code);

        var order = await _context.Order.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        switch (encodeCode)
        {
            case "paid":
                order.isPaid = !order.isPaid;
                break;
            case "ship":
                order.isShip = !order.isShip;
                break;
        }

        _context.Order.Update(order);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(OrderList));
    }

    /// <summary>
    /// 確認是否刪除訂單
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        // 取得訂單物件
        var order = await _context.Order.FirstOrDefaultAsync(m => m.Id == id);

        // 如果找不到訂單，返回NotFound結果
        if (order == null)
        {
            return View(new OrderViewModel());
        }

        // 取得當前使用者物件
        var user = await _userManager.GetUserAsync(User);

        // 判斷當前使用者是否可以查閱訂單
        if (!CanUserViewOrder(order, user))
        {
            return View(new OrderViewModel());
        }

        // 取得訂單項目物件
        order.OrderItem = await _context.OrderItem.Where(p => p.OrderId == id).ToListAsync();
        order.Total = order.OrderItem.Sum(m => m.SubTotal) +
            _DeliveryMethods.First(x => x.Id == order.SelectedDeliveryMethod).Price;
        // 取得商品詳細資料
        var orderItems = GetOrderItems(order.Id);

        OrderViewModel viewModel = new OrderViewModel()
        {
            Order = order,
            CartItems = orderItems,
            DeliveryAddressName = _DeliveryAddresses.First(x => x.Id == order.SelectedDeliveryAddress).Name,
            DeliveryMethodName = _DeliveryMethods.First(x => x.Id == order.SelectedDeliveryMethod).Name
        };

        // 返回View結果
        return View(viewModel);
    }

    // POST: ProductManagement/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // 取得訂單物件
        var order = await _context.Order.FirstOrDefaultAsync(m => m.Id == id);

        // 如果找不到訂單，返回NotFound結果
        if (order == null)
        {
            return View(new OrderViewModel());
        }

        _context.Order.Remove(order);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(OrderList));
    }

    /// <summary>
    /// 結帳
    /// </summary>
    /// <returns></returns>
    public IActionResult Checkout(OrderCheckutDto dto)
    {
        //確認 Session 內存在購物車
        if (SessionHelper.GetObjectFromJson<List<OrderItem>>(HttpContext.Session, "cart") == null)
        {
            return RedirectToAction("Index", "Cart");
        }

        ViewData["DeliveryMethods"] = _DeliveryMethods;

        //建立新的訂單
        var myOrder = new Order()
        {
            UserId = _userManager.GetUserId(User),
            UserName = _userManager.GetUserName(User),
            OrderItem = SessionHelper.GetObjectFromJson<List<OrderItem>>(HttpContext.Session, "cart"),
            SelectedDeliveryAddress = dto.SelectedDeliveryAddress,
            SelectedDeliveryMethod = dto.SelectedDeliveryMethod,
        };
        myOrder.Total = myOrder.OrderItem.Sum(m => m.SubTotal) +
            _DeliveryMethods.First(x => x.Id == dto.SelectedDeliveryMethod).Price;
        ViewBag.CartItems = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

        return View(myOrder);
    }

    /// <summary>
    /// 新增訂單到資料庫
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateOrder(OrderCreateDto dto)
    {
        //新增訂單到資料庫
        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMsg = "購買數量超過庫存，請重新下訂單。";
            return View("ErrorOrder");
        }

        Order order = new Order();
        order.UserId = dto.UserId;
        order.UserName = dto.UserName;
        order.ReceiverName = dto.ReceiverName;
        order.ReceiverPhone = dto.ReceiverPhone;
        order.ReceiverFirstAddress = dto.ReceiverFirstAddress;
        order.ReceiverSecondAddress = dto.ReceiverSecondAddress;
        order.Note = dto.Note ?? string.Empty;
        order.SelectedDeliveryAddress = dto.SelectedDeliveryAddress;
        order.SelectedDeliveryMethod = dto.SelectedDeliveryMethod;
        order.OrderDate = DateTime.UtcNow.AddHours(8);
        order.isPaid = false;
        order.OrderItem = SessionHelper.GetObjectFromJson<List<OrderItem>>(HttpContext.Session, "cart");
        order.Total = order.OrderItem.Sum(m => m.SubTotal) +
            _DeliveryMethods.First(x => x.Id == order.SelectedDeliveryMethod).Price;

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.Add(order);

                StringBuilder errorMsg = new StringBuilder();
                // 扣除庫存
                foreach (var item in order.OrderItem)
                {
                    var dbStock = _context.ProductStyle.Single(x => x.Id == item.ProductStyleId);
                    // 判斷 db 商品款式的庫存，是否大於 購買的數量
                    if (dbStock.Stock >= item.Amount) // 改為所以數量都檢查?!
                    {
                        dbStock.Stock -= item.Amount;
                        _context.Update(dbStock);
                    }
                    else
                    {
                        var dbProduct = _context.Product.Single(x => x.Id == item.ProductId);
                        errorMsg.AppendLine($"{dbProduct.Name} 款式: {dbStock.Name}");
                        errorMsg.AppendLine("<br/>");
                    }
                }
                // 如果有購買數量超過庫存，則跳轉至錯誤頁面
                if (errorMsg.Length > 0)
                {
                    transaction.Rollback();
                    errorMsg.AppendLine("購買數量超過庫存，請重新下訂單。");
                    ViewBag.ErrorMsg = errorMsg.ToString();
                    return View("ErrorOrder");
                }

                // 儲存變更
                _context.SaveChanges();

                // 將庫存 = 0的商品，狀態改為 售完(Status=3)
                // 重新取得最新的訂單資訊
                order = _context.Order.Include(o => o.OrderItem).Single(o => o.Id == order.Id);
                // 篩選 所有 product Id
                var orderProductIds = order.OrderItem.Select(x => x.ProductId);
                foreach (var item in orderProductIds)
                {
                    var stockSum = _context.ProductStyle
                        .Where(p => p.ProductId == item)
                        .Sum(x => x.Stock);

                    if (stockSum == 0)
                    {
                        var p = _context.Product.Single(x => x.Id == item);
                        p.Status = ProductStatus.Sold;
                        _context.Update(p);
                    }
                }

                await _context.SaveChangesAsync();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ViewBag.ErrorMsg = "發生錯誤，請重新下訂單。";
                return View("ErrorOrder");
            }
        }

        SessionHelper.Remove(HttpContext.Session, "cart");

        return RedirectToAction("ReviewOrder", new { Id = order.Id });
    }

    /// <summary>
    /// 取得當前訂單
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public async Task<IActionResult> ReviewOrder(int? Id)
    {
        return RedirectToAction("Details", new { Id = Id, fromReview = 1 });
    }

    /// <summary>
    /// 取得商品詳細資料
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    private List<CartItem> GetOrderItems(int orderId)
    {
        MapperConfiguration config = new MapperConfiguration(cfg =>
                cfg.CreateMap<ProductHistory, Product>());

        //Using automapper
        Mapper mapper = new Mapper(config);

        var orderItems = _context.OrderItem
            .Where(p => p.OrderId == orderId)
            .Select(orderItem => new CartItem(orderItem)
            {
                Product = _context.Product.SingleOrDefault(x => x.Id == orderItem.ProductId)
                      ?? mapper.Map<Product>(_context.ProductHistory.Single(x => x.Id == orderItem.ProductId)),
                ProductStyle = _context.ProductStyle.Single(x => x.Id == orderItem.ProductStyleId)
            })
            .ToList();

        return orderItems;
    }
}
