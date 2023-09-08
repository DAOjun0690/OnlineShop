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

        public DbSet<Product> Product => Set<Product>();
        public DbSet<Category> Category => Set<Category>();
        public DbSet<ProductStyle> ProductStyle => Set<ProductStyle>();
        public DbSet<ProductImage> ProductImage => Set<ProductImage>();
    }
}
