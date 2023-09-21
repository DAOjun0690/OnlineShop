using Microsoft.EntityFrameworkCore;
using OnlineShop.Core.Models;

namespace OnlineShop.Data
{
    public class OnlineShopContext : DbContext
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="options"></param>
        public OnlineShopContext(DbContextOptions<OnlineShopContext> options) : base(options) { }

        public DbSet<Product> Product { get; set; }
        public DbSet<ProductHistory> ProductHistory { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<ProductStyle> ProductStyle { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }

        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }

    }
}
