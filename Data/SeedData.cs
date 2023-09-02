using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Data;

public class SeedData
{
    public static void SeedDatabase(OnlineShopContext context)
    {
        context.Database.Migrate();
        // 檢查 分類 是否有資料
        if (!context.Category.Any())
        {
            List<Category> categorys = new List<Category> {
                new Category()
                {
                    Name = "紙膠帶"
                },
                new Category()
                {
                    Name = "印章"
                }
            };

            context.Category.AddRange(categorys);
            context.SaveChanges();
        }
    }
}