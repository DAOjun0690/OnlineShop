using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using OnlineShop.Core.Models;

namespace OnlineShop.Data
{
    public class OnlineShopContext : IdentityDbContext<OnlineShopUser>
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
        public DbSet<SiteSettings> SiteSettings { get; set; }

        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }

        /// <summary>
        /// Identity DbSet 設定
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
