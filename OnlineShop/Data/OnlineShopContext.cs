using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Data
{
    public class OnlineShopContext : DbContext
    {
        public OnlineShopContext (DbContextOptions<OnlineShopContext> options) : base(options) { }

        public DbSet<Product> Product { get; set; }
        public DbSet<ProductHistory> ProductHistory { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<ProductStyle> ProductStyle { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }

        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }

    }
}
