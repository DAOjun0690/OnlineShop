using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Data;

public class SeedData
{
    public static async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<OnlineShopContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // 如果角色不存在，則新增
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        // 如果管理帳號不存在


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